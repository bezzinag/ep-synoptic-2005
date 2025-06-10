using Microsoft.AspNetCore.Identity;
using ep_synoptic_2005.Models;
using ep_synoptic_2005.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

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
        public IActionResult Create()
        {
            return View();
        }

        // POST: UploadFiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UploadFileViewModel model)
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
                Title = model.Title,
                StoredFileName = uniqueFileName,
                UploadedByUserId = _userManager.GetUserId(User),
                UploadedDate = DateTime.Now
            };

            // ✅ Save to DB using repository
            await _repository.SaveAsync(uploadedFile);

            TempData["Success"] = "File uploaded successfully.";
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Index() // List all uploaded files by the user8
        {
            var files = await _repository.GetFilesByUserAsync(User.Identity.Name);
            return View(files);
        }
    }
}
