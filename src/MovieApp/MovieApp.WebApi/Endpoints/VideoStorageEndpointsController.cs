using Microsoft.AspNetCore.Mvc;
using MovieApp.WebApi.DTOs;
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
        User? creatorUser = null;
        if (reel.CreatorUserId.HasValue)
        {
            creatorUser = await _context.Users.FindAsync(reel.CreatorUserId.Value)
                ?? throw new InvalidOperationException($"User {reel.CreatorUserId.Value} not found.");
        }

        Movie? movie = null;
        if (reel.MovieId.HasValue)
        {
            movie = await _context.Movies.FindAsync(reel.MovieId.Value)
                ?? throw new InvalidOperationException($"Movie {reel.MovieId.Value} not found.");
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
            Movie = movie!,
            CreatorUser = creatorUser!,
        });

        ReelDto insertedReel = inserted.ToDto();
        return Ok(insertedReel);
    }
}
