using Microsoft.AspNetCore.Mvc;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/events")]
public sealed class EventEndpointsController : ControllerBase
{
    private readonly EventRepository _repository;

    public EventEndpointsController(EventRepository repository)
    {
        _repository = repository;
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
}
