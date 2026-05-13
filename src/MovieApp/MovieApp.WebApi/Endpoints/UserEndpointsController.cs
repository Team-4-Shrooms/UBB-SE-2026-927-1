using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/users")]
public sealed class UserEndpointsController : ControllerBase
{
    private readonly IUserService _userService;

    public UserEndpointsController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        return Ok(user?.ToDto());
    }

    [HttpGet("{userId:int}/balance")]
    public async Task<IActionResult> GetBalance(int userId)
    {
        return Ok(await _userService.GetBalanceAsync(userId));
    }

    [HttpPut("{userId:int}/balance")]
    public async Task<IActionResult> UpdateBalance(int userId, [FromBody] UpdateBalanceRequestBody request)
    {
        await _userService.UpdateBalanceAsync(userId, request.NewBalance);
        return Ok();
    }
}
