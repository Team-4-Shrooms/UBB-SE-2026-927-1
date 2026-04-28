using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.DTO.WebAPI;
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
    public IActionResult GetAllEvents()
    {
        return Ok(_repository.GetAllEvents().Select(movieEvent => movieEvent.ToDto()));
    }

    [HttpGet("{eventId:int}")]
    public IActionResult GetEventById(int eventId)
    {
        MovieEventDto? movieEvent = _repository.GetEventById(eventId)?.ToDto();
        return Ok(movieEvent);
    }

    [HttpGet("movie/{movieId:int}")]
    public IActionResult GetEventsForMovie(int movieId)
    {
        return Ok(_repository.GetEventsForMovie(movieId).Select(movieEvent => movieEvent.ToDto()));
    }

    [HttpPost("{eventId:int}/purchase")]
    public IActionResult PurchaseTicket(int eventId, [FromBody] PurchaseTicketRequestBody request)
    {
        try
        {
            _repository.PurchaseTicket(request.UserId, eventId);
            return Ok();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
