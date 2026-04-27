using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("users/{excludedUserId:int}/all-preferences")]
    public async Task<IActionResult> GetAllPreferencesExceptUserAsync(int excludedUserId)
    {
        return Ok(await _repository.GetAllPreferencesExceptUserAsync(excludedUserId));
    }

    [HttpGet("users/{userId:int}/current-preferences")]
    public async Task<IActionResult> GetCurrentUserPreferencesAsync(int userId)
    {
        return Ok(await _repository.GetCurrentUserPreferencesAsync(userId));
    }

    [HttpGet("users/{userId:int}/profile")]
    public async Task<IActionResult> GetUserProfileAsync(int userId)
    {
        return Ok(await _repository.GetUserProfileAsync(userId));
    }

    [HttpGet("users/{excludedUserId:int}/random-user-ids")]
    public async Task<IActionResult> GetRandomUserIdsAsync(int excludedUserId, [FromQuery] int userIdsCount)
    {
        return Ok(await _repository.GetRandomUserIdsAsync(excludedUserId, userIdsCount));
    }

    [HttpGet("users/{userId:int}/username")]
    public async Task<IActionResult> GetUsernameAsync(int userId)
    {
        return Ok(await _repository.GetUsernameAsync(userId));
    }

    [HttpGet("users/{userId:int}/top-preferences")]
    public async Task<IActionResult> GetTopPreferencesWithTitlesAsync(int userId, [FromQuery] int topMoviePreferencesCount)
    {
        return Ok(await _repository.GetTopPreferencesWithTitlesAsync(userId, topMoviePreferencesCount));
    }
}
