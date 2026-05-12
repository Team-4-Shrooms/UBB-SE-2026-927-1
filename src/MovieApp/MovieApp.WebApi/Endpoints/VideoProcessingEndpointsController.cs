using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Features.ReelsEditing;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/video-processing")]

public sealed class VideoProcessingEndpointsController : ControllerBase
{
    private readonly IVideoProcessingService _service;

    public VideoProcessingEndpointsController(IVideoProcessingService service)
    {
        _service = service;
    }

    //    POST /api/video-processing/crop body { videoPath, cropDataJson } →
    //ApplyCropAsync(), returns {outputPath }

    [HttpPost("crop")]
    public async Task<IActionResult> ApplyCropAsync([FromBody] UpdateReelEditsRequestBody request)
    {
        var outputPath = await _service.ApplyCropAsync(request.VideoUrl, request.CropDataJson);
        return Ok(new { outputPath });
    }

    [HttpPost("merge-audio")]
    public async Task<IActionResult> MergeAudioAsync([FromBody] MergeAudioRequestBody request)
    {
        var outputPath = await _service.MergeAudioAsync(request.VideoPath, request.MusicTrackId, request.StartOffsetSec, request.MusicDurationSec, request.VolumePercent);
        return Ok(new { outputPath });
    }
}
