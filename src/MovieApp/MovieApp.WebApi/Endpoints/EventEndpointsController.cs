using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/events")]
public sealed class EventEndpointsController : ControllerBase
{
    private readonly EventRepository _repository;
    private readonly IMovieAppDbContext _context;

    public EventEndpointsController(EventRepository repository, IMovieAppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _repository.GetAllEventsAsync();
        return Ok(events.Select(movieEvent => movieEvent.ToDto()));
    }

    [HttpGet("{eventId:int}")]
    public async Task<IActionResult> GetEventById(int eventId)
    {
        var movieEvent = await _repository.GetEventByIdAsync(eventId);
        return Ok(movieEvent?.ToDto());
    }

    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetEventsForMovie(int movieId)
    {
        var events = await _repository.GetEventsByMovieIdAsync(movieId);
        return Ok(events.Select(movieEvent => movieEvent.ToDto()));
    }

    [HttpGet("{eventId:int}/tickets/{userId:int}")]
    public async Task<IActionResult> UserHasTicket(int eventId, int userId)
    {
        return Ok(await _repository.UserHasTicketAsync(userId, eventId));
    }

    [HttpPost("tickets")]
    public async Task<IActionResult> AddOwnedTicket([FromBody] AddOwnedTicketRequestBody body)
    {
        var user = await _context.Users.FindAsync(body.UserId)
            ?? throw new InvalidOperationException($"User {body.UserId} not found.");
        var movieEvent = await _context.MovieEvents.FindAsync(body.EventId)
            ?? throw new InvalidOperationException($"Event {body.EventId} not found.");
        await _repository.AddOwnedTicketAsync(new OwnedTicket { User = user, Event = movieEvent });
        await _repository.SaveChangesAsync();
        return Ok();
    }
}
