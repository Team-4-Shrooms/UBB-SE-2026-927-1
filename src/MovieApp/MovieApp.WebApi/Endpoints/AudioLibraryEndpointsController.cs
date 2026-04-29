using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebApi.Mappings;
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
        var tracks = await _repository.GetAllTracksAsync();
        return Ok(tracks.Select(track => track.ToDto()));
    }

    [HttpGet("tracks/{musicTrackId:int}")]
    public async Task<IActionResult> GetTrackByIdAsync(int musicTrackId)
    {
        MusicTrackDto? track = (await _repository.GetTrackByIdAsync(musicTrackId))?.ToDto();
        return Ok(track);
    }
}
