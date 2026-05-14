using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebApi.DTOs;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/reels")]
public sealed class ReelEndpointsController : ControllerBase
{
    private readonly IReelService _reelService;

    public ReelEndpointsController(IReelService reelService)
    {
        _reelService = reelService;
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetUserReelsAsync(int userId)
    {
        var reels = await _reelService.GetUserReelsAsync(userId);
        return Ok(reels.Select(reel => reel.ToDto()));
    }

    [HttpGet("{reelId:int}")]
    public async Task<IActionResult> GetReelByIdAsync(int reelId)
    {
        ReelDto? reel = (await _reelService.GetReelByIdAsync(reelId))?.ToDto();
        return Ok(reel);
    }

    [HttpPut("{reelId:int}")]
    public async Task<IActionResult> UpdateReelEditsAsync(int reelId, [FromBody] UpdateReelEditsRequestBody request)
    {
        int affectedRows = await _reelService.UpdateReelEditsAsync(reelId, request.CropDataJson, request.BackgroundMusicId, request.VideoUrl);
        return Ok(affectedRows);
    }

    [HttpDelete("{reelId:int}")]
    public async Task<IActionResult> DeleteReelAsync(int reelId)
    {
        await _reelService.DeleteReelAsync(reelId);
        return Ok();
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadVideoAsync(
        [FromBody] MovieApp.Logic.Features.ReelsUpload.ReelUploadRequest request,
        [FromServices] MovieApp.Logic.Features.ReelsUpload.IVideoStorageService videoStorageService)
    {
        var result = await videoStorageService.UploadVideoAsync(request);
        return Ok(result);
    }
}
