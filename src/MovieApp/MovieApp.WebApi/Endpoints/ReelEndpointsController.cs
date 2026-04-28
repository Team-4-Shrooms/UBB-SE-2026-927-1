using Microsoft.AspNetCore.Mvc;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/reels")]
public sealed class ReelEndpointsController : ControllerBase
{
    private readonly ReelRepository _repository;

    public ReelEndpointsController(ReelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetUserReelsAsync(int userId)
    {
        var reels = await _repository.GetUserReelsAsync(userId);
        return Ok(reels.Select(reel => reel.ToDto()));
    }

    [HttpGet("{reelId:int}")]
    public async Task<IActionResult> GetReelByIdAsync(int reelId)
    {
        ReelDto? reel = (await _repository.GetReelByIdAsync(reelId))?.ToDto();
        return Ok(reel);
    }

    [HttpPut("{reelId:int}")]
    public async Task<IActionResult> UpdateReelEditsAsync(int reelId, [FromBody] UpdateReelEditsRequestBody request)
    {
        int affectedRows = await _repository.UpdateReelEditsAsync(reelId, request.CropDataJson, request.BackgroundMusicId, request.VideoUrl);
        return Ok(affectedRows);
    }

    [HttpDelete("{reelId:int}")]
    public async Task<IActionResult> DeleteReelAsync(int reelId)
    {
        await _repository.DeleteReelAsync(reelId);
        return Ok();
    }
}
