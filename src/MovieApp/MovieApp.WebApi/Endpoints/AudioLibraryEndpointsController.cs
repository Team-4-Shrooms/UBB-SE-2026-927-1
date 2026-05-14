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
[Route("api/audio-library")]
public sealed class AudioLibraryEndpointsController : ControllerBase
{
    private readonly IAudioLibraryService _audioLibraryService;

    public AudioLibraryEndpointsController(IAudioLibraryService audioLibraryService)
    {
        _audioLibraryService = audioLibraryService;
    }

    [HttpGet("tracks")]
    public async Task<IActionResult> GetAllTracksAsync()
    {
        var tracks = await _audioLibraryService.GetAllTracksAsync();
        return Ok(tracks.Select(track => track.ToDto()));
    }

    [HttpGet("tracks/{musicTrackId:int}")]
    public async Task<IActionResult> GetTrackByIdAsync(int musicTrackId)
    {
        MusicTrackDto? track = (await _audioLibraryService.GetTrackByIdAsync(musicTrackId))?.ToDto();
        return Ok(track);
    }
}
