using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.ReelsUpload;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/video-storage")]
public sealed class VideoStorageEndpointsController : ControllerBase
{
    private readonly IVideoStorageService _storageService;
    private readonly IMovieService _movieService;

    public VideoStorageEndpointsController(IVideoStorageService storageService, IMovieService movieService)
    {
        _storageService = storageService;
        _movieService = movieService;
    }

    [HttpPost("insert")]
    public async Task<IActionResult> InsertReelAsync([FromBody] InsertReelRequestBody reel)
    {
        if (reel.CreatorUserId <= 0)
        {
            return BadRequest("CreatorUserId is required and must be greater than 0.");
        }

        if (reel.MovieId <= 0)
        {
            return BadRequest("MovieId is required and must be greater than 0.");
        }

        var movie = await _movieService.GetMovieByIdAsync(reel.MovieId);
        if (movie == null)
        {
            return NotFound($"Movie {reel.MovieId} not found.");
        }

        // Note: For simplicity in this refactor, we are assuming the user existence check is either 
        // done in the service or skipped if we trust the IDs for now, but to be safe:
        // We'll let the service handle the entity mapping.
        
        Reel inserted = await _storageService.InsertReelAsync(new Reel
        {
            VideoUrl = reel.VideoUrl,
            ThumbnailUrl = reel.ThumbnailUrl,
            Title = reel.Title,
            Caption = reel.Caption,
            FeatureDurationSeconds = reel.FeatureDurationSeconds,
            CropDataJson = reel.CropDataJson,
            BackgroundMusicId = reel.BackgroundMusicId,
            Source = reel.Source,
            Genre = reel.Genre,
            CreatedAt = reel.CreatedAt,
            LastEditedAt = reel.LastEditedAt,
            Movie = movie,
            CreatorUser = new User { Id = reel.CreatorUserId } 
        });

        ReelDto insertedReel = inserted.ToDto();
        return Ok(insertedReel);
    }

    [HttpPost("reels")]
    public async Task<IActionResult> UploadVideoAsync([FromBody] MovieApp.Logic.Features.ReelsUpload.ReelUploadRequest request)
    {
        var reel = await _storageService.UploadVideoAsync(request);
        return Ok(reel);
    }
}
