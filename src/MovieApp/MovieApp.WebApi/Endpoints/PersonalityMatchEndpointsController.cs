using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.DTO.WebAPI;
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
}
