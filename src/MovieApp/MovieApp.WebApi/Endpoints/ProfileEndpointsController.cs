using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/profiles")]
public sealed class ProfileEndpointsController : ControllerBase
{
    private readonly ProfileRepository _repository;
    private readonly IMovieAppDbContext _context;

    public ProfileEndpointsController(ProfileRepository repository, IMovieAppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetProfileAsync(int userId)
    {
        UserProfileDto? profile = (await _repository.GetProfileAsync(userId))?.ToDto(userId);
        return Ok(profile);
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
