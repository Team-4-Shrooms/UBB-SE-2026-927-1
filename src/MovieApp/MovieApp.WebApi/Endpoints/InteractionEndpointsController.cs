using Microsoft.AspNetCore.Mvc;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/interactions")]
public sealed class InteractionEndpointsController : ControllerBase
{
    private readonly InteractionRepository _repository;
    private readonly IMovieAppDbContext _context;

    public InteractionEndpointsController(InteractionRepository repository, IMovieAppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> InsertInteractionAsync([FromBody] InsertInteractionRequestBody interaction)
    {
        User user = await _context.Users.FindAsync(interaction.UserId)
            ?? throw new InvalidOperationException($"User {interaction.UserId} not found.");
        Reel reel = await _context.Reels.FindAsync(interaction.ReelId)
            ?? throw new InvalidOperationException($"Reel {interaction.ReelId} not found.");

        await _repository.InsertInteractionAsync(new UserReelInteraction
        {
            IsLiked = interaction.IsLiked,
            WatchDurationSeconds = interaction.WatchDurationSeconds,
            WatchPercentage = interaction.WatchPercentage,
            ViewedAt = interaction.ViewedAt,
            User = user,
            Reel = reel,
        });
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
    public async Task<IActionResult> UpdateViewDataAsync(int userId, int reelId, [FromBody] UpdateViewDataRequestBody request)
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
}
