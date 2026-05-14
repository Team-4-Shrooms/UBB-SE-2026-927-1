using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Features.ReelsFeed;
namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/interactions")]
public sealed class InteractionEndpointsController : ControllerBase
{
    private readonly IReelInteractionService _interactionService;
    private readonly IUserService _userService;
    private readonly IReelService _reelService;

    public InteractionEndpointsController(
        IReelInteractionService interactionService,
        IUserService userService,
        IReelService reelService)
    {
        _interactionService = interactionService;
        _userService = userService;
        _reelService = reelService;
    }

    [HttpPost]
    public async Task<IActionResult> InsertInteractionAsync([FromBody] InsertInteractionRequestBody interaction)
    {
        User? user = await _userService.GetUserByIdAsync(interaction.UserId);
        if (user == null) return NotFound($"User {interaction.UserId} not found.");

        Reel? reel = await _reelService.GetReelByIdAsync(interaction.ReelId);
        if (reel == null) return NotFound($"Reel {interaction.ReelId} not found.");

        await _interactionService.InsertInteractionAsync(new UserReelInteraction
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
        await _interactionService.UpsertInteractionAsync(userId, reelId);
        return Ok();
    }

    [HttpPut("users/{userId:int}/reels/{reelId:int}/like")]
    public async Task<IActionResult> ToggleLikeAsync(int userId, int reelId)
    {
        await _interactionService.ToggleLikeAsync(userId, reelId);
        return Ok();
    }

    [HttpPut("users/{userId:int}/reels/{reelId:int}/view")]
    public async Task<IActionResult> RecordViewAsync(int userId, int reelId, [FromBody] UpdateViewDataRequestBody request)
    {
        await _interactionService.RecordViewAsync(userId, reelId, (double)request.WatchDurationSeconds, (double)request.WatchPercentage);
        return Ok();
    }

    [HttpGet("users/{userId:int}/reels/{reelId:int}")]
    public async Task<IActionResult> GetInteractionAsync(int userId, int reelId)
    {
        var interaction = await _interactionService.GetInteractionAsync(userId, reelId);
        if (interaction == null)
        {
            return Ok(null);
        }
        return Ok(interaction);
    }

    [HttpGet("reels/{reelId:int}/likes")]
    public async Task<IActionResult> GetLikeCountAsync(int reelId)
    {
        return Ok(await _interactionService.GetLikeCountAsync(reelId));
    }

    [HttpGet("reels/{reelId:int}/movie-id")]
    public async Task<IActionResult> GetReelMovieIdAsync(int reelId)
    {
        return Ok(await _interactionService.GetReelMovieIdAsync(reelId));
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetInteractionsForUser(int userId)
    {
        var interactions = await _interactionService.GetInteractionsForUserAsync(userId);
        return Ok(interactions.Select(i => i.ToDto()));
    }
}
