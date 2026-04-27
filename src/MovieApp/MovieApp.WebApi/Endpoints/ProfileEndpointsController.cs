using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/profiles")]
public sealed class ProfileEndpointsController : ControllerBase
{
    private readonly ProfileRepository _repository;

    public ProfileEndpointsController(ProfileRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetProfileAsync(int userId)
    {
        return Ok(await _repository.GetProfileAsync(userId));
    }

    [HttpPost("users/{userId:int}/build")]
    public async Task<IActionResult> BuildProfileFromInteractionsAsync(int userId)
    {
        return Ok(await _repository.BuildProfileFromInteractionsAsync(userId));
    }

    [HttpPut]
    public async Task<IActionResult> UpsertProfileAsync([FromBody] UserProfile profile)
    {
        await _repository.UpsertProfileAsync(profile);
        return Ok();
    }
}
