using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryEndpointsController : ControllerBase
{
    private readonly InventoryRepository _repository;
    private readonly MovieRepository _movieRepository;
    private readonly IMovieAppDbContext _context;

    public InventoryEndpointsController(InventoryRepository repository, MovieRepository movieRepository, IMovieAppDbContext context)
    {
        _repository = repository;
        _movieRepository = movieRepository;
        _context = context;
    }

    [HttpGet("users/{userId:int}/movies")]
    public async Task<IActionResult> GetOwnedMovies(int userId)
    {
        var movies = await _repository.GetOwnedMoviesAsync(userId);
        return Ok(movies.Select(movie => movie.ToDto()));
    }

    [HttpGet("users/{userId:int}/movies/{movieId:int}/ownerships")]
    public async Task<IActionResult> GetMovieOwnerships(int userId, int movieId)
    {
        var ownerships = await _repository.GetMovieOwnershipsAsync(userId, movieId);
        return Ok(ownerships.Select(o => o.ToDto()));
    }

    [HttpPost("movies/ownerships/remove")]
    public async Task<IActionResult> RemoveMovieOwnerships([FromBody] List<int> ownershipIds)
    {
        var ownerships = ownershipIds.Select(id => new OwnedMovie { Id = id }).ToList();
        await _repository.RemoveMovieOwnershipsAsync(ownerships);
        await _repository.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("users/{userId:int}/events/{eventId:int}/tickets")]
    public async Task<IActionResult> GetTicketOwnerships(int userId, int eventId)
    {
        var ownerships = await _repository.GetTicketOwnershipsAsync(userId, eventId);
        return Ok(ownerships.Select(o => o.ToDto()));
    }

    [HttpPost("events/tickets/remove")]
    public async Task<IActionResult> RemoveTicketOwnerships([FromBody] List<int> ownershipIds)
    {
        var ownerships = ownershipIds.Select(id => new OwnedTicket { Id = id }).ToList();
        await _repository.RemoveTicketOwnershipsAsync(ownerships);
        await _repository.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("ownedmovies")]
    public async Task<IActionResult> AddOwnedMovie([FromBody] AddOwnedMovieRequestBody body)
    {
        var user = await _context.Users.FindAsync(body.UserId)
            ?? throw new InvalidOperationException($"User {body.UserId} not found.");
        var movie = await _context.Movies.FindAsync(body.MovieId)
            ?? throw new InvalidOperationException($"Movie {body.MovieId} not found.");
        await _movieRepository.AddOwnedMovieAsync(new OwnedMovie { User = user, Movie = movie });
        await _movieRepository.SaveChangesAsync();
        return Ok();
    }
}
