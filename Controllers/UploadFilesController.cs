using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ep_synoptic_2005.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace ep_synoptic_2005.Controllers
{
    [Authorize]
    public class UploadFilesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadFilesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
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
            if (ModelState.IsValid)
            {
                // Generate unique file name
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.File.FileName);

                // Define the path to store the file (within wwwroot/uploads)
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file to the path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }

                // Store metadata (simulate for now)
                var uploadedFile = new UploadFile
                {
                    Title = model.Title,
                    StoredFileName = uniqueFileName,
                    UploadedByUserId = User.Identity.Name,
                    UploadedDate = DateTime.Now
                };

                // TODO: Add DB save via repository in Section SE1.3 later
                // 

                TempData["Success"] = "File uploaded successfully.";
                return RedirectToAction("Create");
            }

            return View(model);
        }
    }
}