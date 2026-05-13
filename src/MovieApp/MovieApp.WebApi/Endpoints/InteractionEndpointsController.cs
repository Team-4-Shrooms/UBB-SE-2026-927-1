using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Features.ReelsFeed;
using MovieApp.DataLayer.Interfaces.Repositories;
using System.Diagnostics;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/interactions")]
public sealed class InteractionEndpointsController : ControllerBase
{
    private readonly IInteractionRepository _repository;
    private readonly IReelInteractionService _service;
    private readonly IMovieAppDbContext _context;

    public InteractionEndpointsController(IInteractionRepository repository, IReelInteractionService service, IMovieAppDbContext context)
    {
        _repository = repository;
        _service = service;
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
        Debug.WriteLine($"liked in controller");
        await _service.ToggleLikeAsync(userId, reelId);
        return Ok();
    }

    [HttpPut("users/{userId:int}/reels/{reelId:int}/view")]
    public async Task<IActionResult> RecordViewAsync(int userId, int reelId, [FromBody] UpdateViewDataRequestBody request)
    {
        await _service.RecordViewAsync(userId, reelId, (double)request.WatchDurationSeconds, (double)request.WatchPercentage);
        return Ok();
    }

    [HttpGet("users/{userId:int}/reels/{reelId:int}")]
    public async Task<IActionResult> GetInteractionAsync(int userId, int reelId)
    {
        var interaction = await _service.GetInteractionAsync(userId, reelId);
        if (interaction == null)
        {
            return Ok(null);
        }
        return Ok(interaction);
    }

    [HttpGet("reels/{reelId:int}/likes")]
    public async Task<IActionResult> GetLikeCountAsync(int reelId)
    {
        return Ok(await _service.GetLikeCountAsync(reelId));
    }

    [HttpGet("reels/{reelId:int}/movie-id")]
    public async Task<IActionResult> GetReelMovieIdAsync(int reelId)
    {
        return Ok(await _repository.GetReelMovieIdAsync(reelId));
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetInteractionsForUser(int userId)
    {
        var interactions = await _context.UserReelInteractions
            .Include(i => i.User)
            .Include(i => i.Reel)
            .Where(i => i.User.Id == userId)
            .ToListAsync();
        return Ok(interactions.Select(i => i.ToDto()));
    }
}
