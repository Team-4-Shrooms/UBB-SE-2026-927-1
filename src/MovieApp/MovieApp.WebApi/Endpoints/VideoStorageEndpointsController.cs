using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/video-storage")]
public sealed class VideoStorageEndpointsController : ControllerBase
{
    private readonly VideoStorageRepository _repository;
    private readonly IMovieAppDbContext _context;

    public VideoStorageEndpointsController(VideoStorageRepository repository, IMovieAppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpPost("reels")]
    public async Task<IActionResult> InsertReelAsync([FromBody] InsertReelRequestBody reel)
    {
        if (reel.CreatorUserId <= 0)
        {
            return BadRequest("CreatorUserId is required and must be greater than 0.");
        }

        var creatorUser = await _context.Users.FindAsync(reel.CreatorUserId);
        if (creatorUser == null)
        {
            return NotFound($"User {reel.CreatorUserId} not found.");
        }

        if (reel.MovieId <= 0)
        {
            return BadRequest("MovieId is required and must be greater than 0.");
        }

        var movie = await _context.Movies.FindAsync(reel.MovieId);
        if (movie == null)
        {
            return NotFound($"Movie {reel.MovieId} not found.");
        }

        Reel inserted = await _repository.InsertReelAsync(new Reel
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
            CreatorUser = creatorUser,
        });

        ReelDto insertedReel = inserted.ToDto();
        return Ok(insertedReel);
    }
}
