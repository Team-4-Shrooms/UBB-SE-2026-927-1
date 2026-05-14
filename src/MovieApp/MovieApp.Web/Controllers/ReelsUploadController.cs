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


public class ReelsUploadController : Controller
{
    private readonly IVideoStorageService _storageService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMovieService _movieService;

    private const int MaxFileSizeBytes = 100 * 1024 * 1024; // 100 MB
    private const string TitleKey = "Title";
    private const string SuccessMessageKey = "SuccessMessage";
    private const string ErrorMessageKey = "Error";
    private const string IndexView = "Index";
    private const string ReelUploadSuccessMessage = "Reel uploaded successfully!";
    private const string VideoFile = "videoFile";
    private const string ReelsFeedAction = "ReelsFeed";

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
        ViewData[TitleKey] = "Upload Reel";
        var movies = await _movieService.GetAllMoviesAsync();
        ViewBag.AvailableMovies = movies;

        return View(new ReelUploadForm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> Upload(IFormFile videoFile, ReelUploadForm form)
    {
        if (videoFile == null || videoFile.Length == 0)
        {
            TempData[ErrorMessageKey] = "Please select a video file to upload.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            TempData[ErrorMessageKey] = "Please fill out all required fields.";
            return RedirectToAction(nameof(Index));
        }

        string tempPath = Path.GetTempFileName();

        try
        {
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            int userId = _currentUserService.UserId;

            var request = new ReelUploadRequest
            {
                LocalFilePath = tempPath,
                Title = form.Title ?? string.Empty,
                Caption = form.Description ?? string.Empty,
                UploaderUserId = userId,
                MovieId = form.MovieId
            };

            await _storageService.UploadVideoAsync(request);

            TempData[SuccessMessageKey] = ReelUploadSuccessMessage;
            return RedirectToAction(IndexView, ReelsFeedAction);
        }
        catch (Exception exception)
        {
            TempData[ErrorMessageKey] = $"Upload failed: {exception.Message}";
            return RedirectToAction(nameof(Index));
        }
        finally
        {
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }
        }
    }
}
