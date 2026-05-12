using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/inventory")]
public sealed class InventoryEndpointsController : ControllerBase
{
    private readonly InventoryRepository _repository;
    private readonly MovieRepository _movieRepository;
    private readonly IMovieAppDbContext _context;
    private readonly IInventoryService _inventoryService;

    public InventoryEndpointsController(InventoryRepository repository, MovieRepository movieRepository, IMovieAppDbContext context, IInventoryService inventoryService)
    {
        _repository = repository;
        _movieRepository = movieRepository;
        _context = context;
        _inventoryService = inventoryService;
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

    [HttpGet("users/{userId:int}/tickets")]
    public async Task<IActionResult> GetOwnedTickets(int userId)
    {
        var tickets = await _inventoryService.GetOwnedTicketsAsync(userId);
        return Ok(tickets.Select(t => new
        {
            t.Id,
            t.PurchaseDate,
            EventId = t.Event?.Id ?? 0,
            EventTitle = t.Event?.Title ?? string.Empty,
            EventDate = t.Event?.Date,
            EventLocation = t.Event?.Location ?? string.Empty,
        }));
    }

    [HttpPost("remove-movie")]
    public async Task<IActionResult> RemoveOwnedMovie([FromBody] RemoveMovieRequestBody body)
    {
        await _inventoryService.RemoveOwnedMovieAsync(body.UserId, body.MovieId);
        return Ok();
    }

    [HttpPost("remove-ticket")]
    public async Task<IActionResult> RemoveOwnedTicket([FromBody] RemoveTicketRequestBody body)
    {
        await _inventoryService.RemoveOwnedTicketAsync(body.UserId, body.EventId);
        return Ok();
    }
}
