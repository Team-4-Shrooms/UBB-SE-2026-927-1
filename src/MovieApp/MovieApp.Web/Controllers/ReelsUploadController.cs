using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Features.ReelsUpload;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.ViewModels.ReelUpload;

namespace MovieApp.Mvc.Features.ReelsUpload.Controllers;

//[Authorize]
public class ReelsUploadController : Controller
{
    private readonly IVideoStorageService _storageService;
    private readonly ICurrentUserService _currentUserService;

    public ReelsUploadController(IVideoStorageService storageService, ICurrentUserService currentUserService)
    {
        _storageService = storageService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Upload Reel";
        return View(new ReelUploadForm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB Limit
    public async Task<IActionResult> Upload(IFormFile videoFile, ReelUploadForm form)
    {
        // 1. Validate the file is present
        if (videoFile == null || videoFile.Length == 0)
        {
            ModelState.AddModelError("videoFile", "Please select a valid video file.");
            return View("Index", form);
        }

        if (!ModelState.IsValid) return View("Index", form);

        string tempPath = Path.GetTempFileName();

        try
        {
            // 2. Save IFormFile to the temp path
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            int userId = _currentUserService.UserId;

            // 3. Map to your logic layer's exact request model
            var request = new ReelUploadRequest
            {
                LocalFilePath = tempPath,
                Title = form.Title ?? string.Empty,
                Caption = form.Description ?? string.Empty, // Mapping form Description to request Caption
                UploaderUserId = userId,
                MovieId = null
            };

            // 4. Pass to the storage service
            await _storageService.UploadVideoAsync(request);

            // 5. On success, set TempData and redirect
            TempData["SuccessMessage"] = "Reel uploaded successfully!";
            return RedirectToAction("Index", "ReelsFeed");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Upload failed: {ex.Message}");
            return View("Index", form);
        }
        finally
        {
            // 6. ALWAYS delete the temp file, even if the service throws an exception
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }
        }
    }
}
