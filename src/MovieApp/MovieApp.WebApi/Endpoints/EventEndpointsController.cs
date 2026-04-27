using Microsoft.AspNetCore.Mvc;
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
        return Ok(_repository.GetAllEvents());
    }

    [HttpGet("{eventId:int}")]
    public IActionResult GetEventById(int eventId)
    {
        return Ok(_repository.GetEventById(eventId));
    }

    [HttpGet("movie/{movieId:int}")]
    public IActionResult GetEventsForMovie(int movieId)
    {
        return Ok(_repository.GetEventsForMovie(movieId));
    }

    [HttpPost("{eventId:int}/purchase")]
    public IActionResult PurchaseTicket(int eventId, [FromBody] PurchaseTicketRequest request)
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

    public sealed record PurchaseTicketRequest(int UserId);
}
