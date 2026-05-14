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
[Route("api/events")]
public sealed class EventEndpointsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventEndpointsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEvents([FromQuery] int? movieId)
    {
        var events = movieId.HasValue
            ? await _eventService.GetEventsByMovieIdAsync(movieId.Value)
            : await _eventService.GetAvailableEventsAsync();
        return Ok(events.Select(movieEvent => movieEvent.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        var movieEvent = await _eventService.GetEventByIdAsync(id);
        return Ok(movieEvent?.ToDto());
    }

    [HttpGet("{id:int}/tickets/{userId:int}")]
    public async Task<IActionResult> UserHasTicket(int id, int userId)
    {
        return Ok(await _eventService.UserHasTicketAsync(userId, id));
    }

    [HttpPost("{id:int}/purchase-ticket")]
    public async Task<IActionResult> PurchaseTicketAsync(int id, [FromBody] PurchaseTicketRequestBody body)
    {
        try
        {
            await _eventService.PurchaseTicketAsync(body.UserId, id);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred: " + ex.Message);
        }
    }
}
