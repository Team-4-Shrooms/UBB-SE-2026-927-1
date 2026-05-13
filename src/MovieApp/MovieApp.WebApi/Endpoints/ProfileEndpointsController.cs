using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/profiles")]
public sealed class ProfileEndpointsController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly IUserService _userService;

    public ProfileEndpointsController(IProfileService profileService, IUserService userService)
    {
        _profileService = profileService;
        _userService = userService;
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetProfileAsync(int userId)
    {
        var profile = await _profileService.BuildProfileFromInteractionsAsync(userId);
        return Ok(profile.ToDto(userId));
    }

    [HttpPost]
    public async Task<IActionResult> AddProfileAsync([FromBody] UpsertProfileRequestBody body)
    {
        var user = await _userService.GetUserByIdAsync(body.UserId)
            ?? throw new InvalidOperationException($"User {body.UserId} not found.");

        var profile = body.ToModel();
        profile.User = user;
        await _profileService.AddProfileAsync(profile);
        return Ok();
    }
}
