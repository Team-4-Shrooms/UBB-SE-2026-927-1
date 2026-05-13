using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Features.PersonalityMatch;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using System.Threading.Tasks;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/personality-match")]
public sealed class PersonalityMatchEndpointsController : ControllerBase
{
    private readonly PersonalityMatchRepository _repository;
    private readonly IPersonalityMatchService _personalityMatchService;
    private readonly IPersonalityMatchingService _personalityMatchingService;

    public PersonalityMatchEndpointsController(
        PersonalityMatchRepository repository,
        IPersonalityMatchService personalityMatchService,
        IPersonalityMatchingService personalityMatchingService)
    {
        _repository = repository;
        _personalityMatchService = personalityMatchService;
        _personalityMatchingService = personalityMatchingService;
    }

    [HttpGet("users/{userId:int}/current-preferences")]
    public async Task<IActionResult> GetCurrentUserPreferencesAsync(int userId)
    {
        var preferences = await _repository.GetCurrentUserPreferencesAsync(userId);
        return Ok(preferences.Select(preference => preference.ToDto()));
    }

    [HttpGet("users/{userId:int}/profile")]
    public async Task<IActionResult> GetUserProfileAsync(int userId)
    {
        UserProfileDto? profile = (await _personalityMatchingService.GetUserProfileAsync(userId))?.ToDto(userId);
        return Ok(profile);
    }

    [HttpGet("users/{excludedUserId:int}/random-user-ids")]
    public async Task<IActionResult> GetRandomUserIdsAsync(int excludedUserId, [FromQuery] int userIdsCount)
    {
        return Ok(await _repository.GetRandomUserIdsAsync(excludedUserId, userIdsCount));
    }

    [HttpGet("users/{excludedUserId:int}/others-preferences")]
    public async Task<IActionResult> GetAllPreferencesExceptUser(int excludedUserId)
    {
        var preferences = await _personalityMatchService.GetAllPreferencesGroupedAsync(excludedUserId);
        var flat = preferences.SelectMany(kvp => kvp.Value).Select(p => p.ToDto());
        return Ok(flat);
    }

    [HttpGet("users/{userId:int}/top-preferences")]
    public async Task<IActionResult> GetTopPreferencesWithTitlesAsync(int userId, [FromQuery] int count)
    {
        var prefs = await _personalityMatchingService.GetTopMoviePreferencesAsync(userId, count);
        return Ok(prefs);
    }

    [HttpGet("users/{userId:int}/username")]
    public async Task<IActionResult> GetUsernameAsync(int userId)
    {
        return Ok(await _personalityMatchService.GetUsernameAsync(userId));
    }
}
