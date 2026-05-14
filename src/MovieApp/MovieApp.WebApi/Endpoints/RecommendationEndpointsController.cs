using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.Logic.Features.ReelsFeed;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/recommendations")]
public sealed class RecommendationEndpointsController : ControllerBase
{
    private readonly IRecommendationService _service;

    public RecommendationEndpointsController(IRecommendationService service)
    {
        _service = service;
    }

    [HttpGet("users/{userId:int}/has-preferences")]
    public async Task<IActionResult> UserHasPreferencesAsync(int userId)
    {
        return Ok(await _service.UserHasPreferencesAsync(userId));
    }

    [HttpGet("reels")]
    public async Task<IActionResult> GetAllReelsAsync()
    {
        var reels = await _service.GetAllReelsAsync();
        return Ok(reels.Select(reel => reel.ToDto()));
    }

    [HttpGet("users/{userId:int}/recommended-reels/count={n:int}")]
    public async Task<IActionResult> GetRecommendedReelsAsync(int userId, int n)
    {
        var reels = await _service.GetRecommendedReelsAsync(userId, n);
        return Ok(reels.Select(reel => reel.ToDto()));
    }

    [HttpGet("users/{userId:int}/preference-scores")]
    public async Task<IActionResult> GetUserPreferenceScoresAsync(int userId)
    {
        return Ok(await _service.GetUserPreferenceScoresAsync(userId));
    }

    [HttpGet("like-counts")]
    public async Task<IActionResult> GetAllLikeCountsAsync()
    {
        return Ok(await _service.GetAllLikeCountsAsync());
    }

    [HttpGet("likes/within/{days:int}")]
    public async Task<IActionResult> GetLikesWithinDaysAsync(int days)
    {
        var interactions = await _service.GetLikesWithinDaysAsync(days);
        return Ok(interactions.Select(interaction => interaction.ToDto()));
    }
}
