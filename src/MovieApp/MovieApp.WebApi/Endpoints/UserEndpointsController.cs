using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/users")]
public sealed class UserEndpointsController : ControllerBase
{
    private readonly UserRepository _repository;

    public UserEndpointsController(UserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        return Ok(user?.ToDto());
    }

    [HttpGet("{userId:int}/balance")]
    public async Task<IActionResult> GetBalance(int userId)
    {
        return Ok(await _repository.GetBalanceAsync(userId));
    }

    [HttpPut("{userId:int}/balance")]
    public async Task<IActionResult> UpdateBalance(int userId, [FromBody] UpdateBalanceRequestBody request)
    {
        await _repository.UpdateBalanceAsync(userId, request.NewBalance);
        return Ok();
    }
}
