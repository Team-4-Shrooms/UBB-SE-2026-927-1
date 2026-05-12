using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/profiles")]
public sealed class ProfileEndpointsController : ControllerBase
{
    private readonly ProfileRepository _repository;
    private readonly IMovieAppDbContext _context;
    private readonly IProfileService _profileService;

    public ProfileEndpointsController(ProfileRepository repository, IMovieAppDbContext context, IProfileService profileService)
    {
        _repository = repository;
        _context = context;
        _profileService = profileService;
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
        var user = await _context.Users.FindAsync(body.UserId)
            ?? throw new InvalidOperationException($"User {body.UserId} not found.");
        var profile = body.ToModel();
        profile.User = user;
        await _repository.AddProfileAsync(profile);
        await _repository.SaveChangesAsync();
        return Ok();
    }
}
