using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/preferences")]
public sealed class PreferenceEndpointsController : ControllerBase
{
    private readonly PreferenceRepository _repository;

    public PreferenceEndpointsController(PreferenceRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("users/{userId:int}/movies/{movieId:int}/exists")]
    public async Task<IActionResult> PreferenceExistsAsync(int userId, int movieId)
    {
        return Ok(await _repository.PreferenceExistsAsync(userId, movieId));
    }

    [HttpPost]
    public async Task<IActionResult> InsertPreferenceAsync([FromBody] InsertPreferenceRequestBody request)
    {
        await _repository.InsertPreferenceAsync(request.UserId, request.MovieId, request.Score);
        return Ok();
    }

    [HttpPut("users/{userId:int}/movies/{movieId:int}/boost")]
    public async Task<IActionResult> UpdatePreferenceAsync(int userId, int movieId, [FromBody] UpdatePreferenceRequestBody request)
    {
        await _repository.UpdatePreferenceAsync(userId, movieId, request.Boost);
        return Ok();
    }
}
