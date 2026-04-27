using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/video-storage")]
public sealed class VideoStorageEndpointsController : ControllerBase
{
    private readonly VideoStorageRepository _repository;

    public VideoStorageEndpointsController(VideoStorageRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("reels")]
    public async Task<IActionResult> InsertReelAsync([FromBody] Reel reel)
    {
        return Ok(await _repository.InsertReelAsync(reel));
    }
}
