using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/audio-library")]
public sealed class AudioLibraryEndpointsController : ControllerBase
{
    private readonly AudioLibraryRepository _repository;

    public AudioLibraryEndpointsController(AudioLibraryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("tracks")]
    public async Task<IActionResult> GetAllTracksAsync()
    {
        return Ok(await _repository.GetAllTracksAsync());
    }

    [HttpGet("tracks/{musicTrackId:int}")]
    public async Task<IActionResult> GetTrackByIdAsync(int musicTrackId)
    {
        return Ok(await _repository.GetTrackByIdAsync(musicTrackId));
    }
}
