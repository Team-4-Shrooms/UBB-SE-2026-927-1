using Microsoft.AspNetCore.Mvc;
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
        return Ok(await _repository.GetUserReelsAsync(userId));
    }

    [HttpGet("{reelId:int}")]
    public async Task<IActionResult> GetReelByIdAsync(int reelId)
    {
        return Ok(await _repository.GetReelByIdAsync(reelId));
    }

    [HttpPut("{reelId:int}")]
    public async Task<IActionResult> UpdateReelEditsAsync(int reelId, [FromBody] UpdateReelEditsRequest request)
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

    public sealed record UpdateReelEditsRequest(string CropDataJson, int? BackgroundMusicId, string VideoUrl);
}
