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
    private readonly IMovieService _movieService; // 1. Added the private field

    // 2. Updated constructor to inject IMovieService
    public ReelsUploadController(
        IVideoStorageService storageService, 
        ICurrentUserService currentUserService,
        IMovieService movieService)
    {
        _storageService = storageService;
        _currentUserService = currentUserService;
        _movieService = movieService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Upload Reel";
        
        // 3. Fetch movies and pass them to the View via ViewBag
        var movies = await _movieService.GetAllMoviesAsync();
        ViewBag.AvailableMovies = movies;

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
                Caption = form.Description ?? string.Empty,
                UploaderUserId = userId,
                MovieId = form.MovieId // Use the ID selected by the user
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
            
            // Re-populate movies in case of error so the dropdown isn't empty
            ViewBag.AvailableMovies = await _movieService.GetAllMoviesAsync();
            return View("Index", form);
        }
        finally
        {
            // 6. ALWAYS delete the temp file
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }
        }
    }
}
