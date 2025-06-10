using ep_synoptic_2005.Filters;
using ep_synoptic_2005.Models;
using ep_synoptic_2005.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ep_synoptic_2005.Controllers
{
    [Authorize]
    public class UploadFilesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUploadFileRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public UploadFilesController(
            IWebHostEnvironment webHostEnvironment,
            IUploadFileRepository repository,
            UserManager<IdentityUser> userManager)
        {
            _webHostEnvironment = webHostEnvironment;
            _repository = repository;
            _userManager = userManager;
        }

        // GET: UploadFiles/Create
        [HttpGet]
        public async Task<IActionResult> InjectedTest([FromServices] IUploadFileRepository repo) // is this working?// in my code//
        {
            var userId = _userManager.GetUserId(User);
            var files = await repo.GetFilesByUserAsync(userId);

            TempData["Success"] = $"Method injection succeeded. Found {files.Count} file(s).";
            return RedirectToAction("Create");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // POST: UploadFiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UploadFileViewModel model) // method injection
        {
            if (!ModelState.IsValid)
                return View(model);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.File.FileName);

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");


            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(fileStream);
            }

            var uploadedFile = new UploadFile
            {
                Title = Path.GetFileName(model.File.FileName),
                StoredFileName = uniqueFileName,
                UploadedByUserId = _userManager.GetUserId(User),
                UploadedDate = DateTime.Now
            };

            // ✅ Save to DB using repository
            await _repository.SaveAsync(uploadedFile);

            TempData["Success"] = "File uploaded successfully.";
            return RedirectToAction("Create");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var files = await _repository.GetFilesByUserAsync(userId);
            return View(files);
        }

        [Authorize]
        [ServiceFilter(typeof(OwnershipFilter))]
        public async Task<IActionResult> Download(int id)
        {
            var file = await _repository.GetByIdAsync(id);
            if (file == null) return NotFound();

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", file.StoredFileName);
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var contentType = "application/octet-stream";
            return PhysicalFile(filePath, contentType, $"{file.Title}{Path.GetExtension(file.StoredFileName)}"); //better

        }





    }
}
