using ep_synoptic_2005.Filters;
using ep_synoptic_2005.Models;
using ep_synoptic_2005.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


// <summary>  UploadFilesController.cs
//  This controller handles file uploads, downloads, and listing of uploaded files for authenticated users.
//  It uses dependency injection to access the file repository and user management services.
// </summary>

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
        // <summary>
        // This action method demonstrates method injection by retrieving the file repository from the service container.
        // It fetches files uploaded by the current user and displays a success message.
        // </summary>
        [HttpGet]
        public async Task<IActionResult> InjectedTest([FromServices] IUploadFileRepository repo)
        {
            var userId = _userManager.GetUserId(User);
            var files = await repo.GetFilesByUserAsync(userId);

            TempData["Success"] = $"Method injection succeeded. Found {files.Count} file(s).";
            return RedirectToAction("Create");
        }
        // <summary>
        // This action method displays a form for uploading files.
        // It returns a view with an empty UploadFileViewModel for the user to fill out.
        // </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        // <summary>
        // This action method handles the file upload process.
        // It validates the model, saves the file to the server, and records the upload in the database.
        // If successful, it redirects to the Create view with a success message.
        // </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UploadFileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var originalFileName = Path.GetFileName(model.File.FileName);
            var uniqueFileName = Guid.NewGuid() + "_" + originalFileName;
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
                OriginalFileName = originalFileName,
                StoredFileName = uniqueFileName,
                UploadedByUserId = _userManager.GetUserId(User),
                UploadedDate = DateTime.Now
            };

            await _repository.SaveAsync(uploadedFile);

            TempData["Success"] = "File uploaded successfully.";
            return RedirectToAction("Create");
        }
        // <summary>
        // This action method lists all files uploaded by the current user.
        // It retrieves the files from the repository and returns them to the Index view.
        // </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var files = await _repository.GetFilesByUserAsync(userId);
            return View(files);
        }
        // <summary>
        // This action method allows the user to download a file by its ID.
        // It checks if the file exists and if the user has permission to access it, then serves the file for download.
        // </summary>
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
    }
}
