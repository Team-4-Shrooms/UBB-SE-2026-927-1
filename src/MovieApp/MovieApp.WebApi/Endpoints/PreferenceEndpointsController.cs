using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Features.MovieSwipe;
using MovieApp.WebApi.Mappings;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using System.Threading.Tasks;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/preferences")]
public sealed class PreferenceEndpointsController : ControllerBase
{
    private readonly IPreferenceService _preferenceService;
    private readonly IMovieCardFeedService _movieCardFeedService;

    public PreferenceEndpointsController(IPreferenceService preferenceService, IMovieCardFeedService movieCardFeedService)
    {
        _preferenceService = preferenceService;
        _movieCardFeedService = movieCardFeedService;
    }

    [HttpGet("users/{userId:int}/movies/{movieId:int}/exists")]
    public async Task<IActionResult> PreferenceExistsAsync(int userId, int movieId)
    {
        return Ok(await _preferenceService.PreferenceExistsAsync(userId, movieId));
    }

    [HttpPost]
    public async Task<IActionResult> InsertPreferenceAsync([FromBody] InsertPreferenceRequestBody request)
    {
        await _preferenceService.InsertPreferenceAsync(request.UserId, request.MovieId, request.Score);
        return Ok();
    }

    [HttpPut("users/{userId:int}/movies/{movieId:int}/boost")]
    public async Task<IActionResult> UpdatePreferenceAsync(int userId, int movieId, [FromBody] UpdatePreferenceRequestBody request)
    {
        await _preferenceService.UpdatePreferenceAsync(userId, movieId, request.Boost);
        return Ok();
    }

    [HttpGet("users/{userId:int}/feed")]
    public async Task<IActionResult> GetMovieFeedAsync(int userId, [FromQuery] int count)
    {
        var feed = await _movieCardFeedService.FetchMovieFeedAsync(userId, count);
        return Ok(feed.Select(movie => movie.ToDto()));
    }
}
