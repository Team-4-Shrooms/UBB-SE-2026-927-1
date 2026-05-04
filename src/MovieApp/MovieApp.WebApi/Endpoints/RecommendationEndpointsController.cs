using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/recommendations")]
public sealed class RecommendationEndpointsController : ControllerBase
{
    private readonly RecommendationRepository _repository;

    public RecommendationEndpointsController(RecommendationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("users/{userId:int}/has-preferences")]
    public async Task<IActionResult> UserHasPreferencesAsync(int userId)
    {
        return Ok(await _repository.UserHasPreferencesAsync(userId));
    }

    [HttpGet("reels")]
    public async Task<IActionResult> GetAllReelsAsync()
    {
        var reels = await _repository.GetAllReelsAsync();
        return Ok(reels.Select(reel => reel.ToDto()));
    }

    [HttpGet("users/{userId:int}/preference-scores")]
    public async Task<IActionResult> GetUserPreferenceScoresAsync(int userId)
    {
        return Ok(await _repository.GetUserPreferenceScoresAsync(userId));
    }

    [HttpGet("like-counts")]
    public async Task<IActionResult> GetAllLikeCountsAsync()
    {
        return Ok(await _repository.GetAllLikeCountsAsync());
    }

    [HttpGet("likes/within/{days:int}")]
    public async Task<IActionResult> GetLikesWithinDaysAsync(int days)
    {
        var interactions = await _repository.GetLikesWithinDaysAsync(days);
        return Ok(interactions.Select(interaction => interaction.ToDto()));
    }
}
