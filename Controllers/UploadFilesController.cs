using ep_synoptic_2005.Filters;
using ep_synoptic_2005.Models;
using ep_synoptic_2005.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// <summary>
// This controller handles file uploads, downloads, and management for logged-in users.
// It provides actions for uploading files, viewing uploaded files, downloading files, and testing method injection.
// The controller uses dependency injection to access the file repository and user management services.
// </summary>
namespace ep_synoptic_2005.Controllers
{
    [Authorize]
    public class UploadFilesController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUploadFileRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor to inject dependencies
        public UploadFilesController(
            IWebHostEnvironment webHostEnvironment,
            IUploadFileRepository repository,
            UserManager<IdentityUser> userManager)
        {
            _webHostEnvironment = webHostEnvironment;
            _repository = repository;
            _userManager = userManager;
        }

        // GET: Display the file upload form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Handle file upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UploadFileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "You must be logged in to upload a file.";
                return RedirectToAction("Create");
            }

            var originalFileName = Path.GetFileName(model.File.FileName);
            var uniqueFileName = Guid.NewGuid() + "_" + originalFileName;
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(fileStream);
            }

            var uploadedFile = new UploadFile
            {
                Title = model.Title,
                OriginalFileName = originalFileName,
                StoredFileName = uniqueFileName,
                UploadedByUserId = userId,
                UploadedDate = DateTime.UtcNow
            };

            await _repository.SaveAsync(uploadedFile);

            TempData["Success"] = "File uploaded successfully.";
            return RedirectToAction("Create");
        }

        // GET: List all uploaded files for the logged-in user
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var files = await _repository.GetFilesByUserAsync(userId);
            return View(files);
        }

        // GET: Download a specific file by ID
        [Authorize]
        [ServiceFilter(typeof(OwnershipFilter))]
        public async Task<IActionResult> Download(int id)
        {
            var file = await _repository.GetByIdAsync(id);
            if (file == null)
                return NotFound();

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", file.StoredFileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var contentType = "application/octet-stream";
            return PhysicalFile(filePath, contentType, file.OriginalFileName);
        }

        // GET: Test method injection using FromServices attribute
        [HttpGet]
        public async Task<IActionResult> InjectedTest([FromServices] IUploadFileRepository repo)
        {
            var userId = _userManager.GetUserId(User);
            var files = await repo.GetFilesByUserAsync(userId);

            TempData["Success"] = $"Method injection succeeded. Found {files.Count} file(s).";
            return RedirectToAction("Create");
        }
    }
}
