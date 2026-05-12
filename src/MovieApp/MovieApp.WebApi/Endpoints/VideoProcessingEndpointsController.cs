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

    [HttpPost("crop")]
    public async Task<IActionResult> ApplyCropAsync([FromBody] UpdateReelEditsRequestBody request)
    {
        System.Diagnostics.Debug.WriteLine($"RAW API RESPONSE: {request.VideoUrl}, {request.CropDataJson}");
        var outputPath = await _service.ApplyCropAsync(request.VideoUrl, request.CropDataJson);
        System.Diagnostics.Debug.WriteLine($"output: {outputPath}, {Ok(outputPath).StatusCode}");

        return Ok(new {outputPath});
    }

    [HttpPost("merge-audio")]
    public async Task<IActionResult> MergeAudioAsync([FromBody] MergeAudioRequestBody request)
    {
        System.Diagnostics.Debug.WriteLine($"RAW API RESPONSE: {request.VideoPath}, {request.MusicTrackId}");
        var outputPath = await _service.MergeAudioAsync(request.VideoPath, request.MusicTrackId, request.StartOffsetSec, request.MusicDurationSec, request.VolumePercent);
        return Ok(new { outputPath });
    }
}
