using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/interactions")]
public sealed class InteractionEndpointsController : ControllerBase
{
    private readonly InteractionRepository _repository;

    public InteractionEndpointsController(InteractionRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> InsertInteractionAsync([FromBody] UserReelInteraction interaction)
    {
        await _repository.InsertInteractionAsync(interaction);
        return Ok();
    }

    [HttpPost("users/{userId:int}/reels/{reelId:int}")]
    public async Task<IActionResult> UpsertInteractionAsync(int userId, int reelId)
    {
        await _repository.UpsertInteractionAsync(userId, reelId);
        return Ok();
    }

    [HttpPut("users/{userId:int}/reels/{reelId:int}/like")]
    public async Task<IActionResult> ToggleLikeAsync(int userId, int reelId)
    {
        await _repository.ToggleLikeAsync(userId, reelId);
        return Ok();
    }

    [HttpPut("users/{userId:int}/reels/{reelId:int}/view")]
    public async Task<IActionResult> UpdateViewDataAsync(int userId, int reelId, [FromBody] UpdateViewDataRequest request)
    {
        await _repository.UpdateViewDataAsync(userId, reelId, request.WatchDurationSeconds, request.WatchPercentage);
        return Ok();
    }

    [HttpGet("users/{userId:int}/reels/{reelId:int}")]
    public async Task<IActionResult> GetInteractionAsync(int userId, int reelId)
    {
        return Ok(await _repository.GetInteractionAsync(userId, reelId));
    }

    [HttpGet("reels/{reelId:int}/likes")]
    public async Task<IActionResult> GetLikeCountAsync(int reelId)
    {
        return Ok(await _repository.GetLikeCountAsync(reelId));
    }

    [HttpGet("reels/{reelId:int}/movie-id")]
    public async Task<IActionResult> GetReelMovieIdAsync(int reelId)
    {
        return Ok(await _repository.GetReelMovieIdAsync(reelId));
    }

    public sealed record UpdateViewDataRequest(decimal WatchDurationSeconds, decimal WatchPercentage);
}
