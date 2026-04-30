using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/personality-match")]
public sealed class PersonalityMatchEndpointsController : ControllerBase
{
    private readonly PersonalityMatchRepository _repository;

    public PersonalityMatchEndpointsController(PersonalityMatchRepository repository)
    {
        _repository = repository;
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
        UserProfileDto? profile = (await _repository.GetUserProfileAsync(userId))?.ToDto(userId);
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
        var preferences = await _repository.GetAllPreferencesExceptUserAsync(excludedUserId);
        return Ok(preferences.Select(p => p.ToDto()));
    }

    [HttpGet("users/{userId:int}/top-preferences")]
    public async Task<IActionResult> GetTopPreferencesWithTitlesAsync(int userId, [FromQuery] int count)
    {
        return Ok(await _repository.GetTopPreferencesWithTitlesAsync(userId, count));
    }

    [HttpGet("users/{userId:int}/username")]
    public async Task<IActionResult> GetUsernameAsync(int userId)
    {
        return Ok(await _repository.GetUsernameAsync(userId));
    }
}
