using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.DTO.WebAPI;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/profiles")]
public sealed class ProfileEndpointsController : ControllerBase
{
    private readonly ProfileRepository _repository;
    private readonly IMovieAppDbContext _context;

    public ProfileEndpointsController(ProfileRepository repository, IMovieAppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetProfileAsync(int userId)
    {
        UserProfileDto? profile = (await _repository.GetProfileAsync(userId))?.ToDto(userId);
        return Ok(profile);
    }

    [HttpPost("users/{userId:int}/build")]
    public async Task<IActionResult> BuildProfileFromInteractionsAsync(int userId)
    {
        return Ok((await _repository.BuildProfileFromInteractionsAsync(userId)).ToDto(userId));
    }

    [HttpPut]
    public async Task<IActionResult> UpsertProfileAsync([FromBody] UpsertProfileRequestBody profile)
    {
        User user = await _context.Users.FindAsync(profile.UserId)
            ?? throw new InvalidOperationException($"User {profile.UserId} not found.");

        await _repository.UpsertProfileAsync(new UserProfile
        {
            TotalLikes = profile.TotalLikes,
            TotalWatchTimeSeconds = profile.TotalWatchTimeSeconds,
            AverageWatchTimeSeconds = profile.AverageWatchTimeSeconds,
            TotalClipsViewed = profile.TotalClipsViewed,
            LikeToViewRatio = profile.LikeToViewRatio,
            LastUpdated = profile.LastUpdated,
            User = user,
        });
        return Ok();
    }
}
