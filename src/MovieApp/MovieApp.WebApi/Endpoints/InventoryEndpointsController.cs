using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/inventory")]
public sealed class InventoryEndpointsController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryEndpointsController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("users/{userId:int}/movies")]
    public async Task<IActionResult> GetOwnedMovies(int userId)
    {
        var movies = await _inventoryService.GetOwnedMoviesAsync(userId);
        return Ok(movies.Select(movie => movie.ToDto()));
    }

    [HttpGet("users/{userId:int}/movies/{movieId:int}/ownerships")]
    public async Task<IActionResult> GetMovieOwnerships(int userId, int movieId)
    {
        var ownerships = await _inventoryService.GetMovieOwnershipsAsync(userId, movieId);
        return Ok(ownerships.Select(o => o.ToDto()));
    }

    [HttpPost("movies/ownerships/remove")]
    public async Task<IActionResult> RemoveMovieOwnerships([FromBody] List<int> ownershipIds)
    {
        await _inventoryService.RemoveMovieOwnershipsAsync(ownershipIds);
        return Ok();
    }

    [HttpGet("users/{userId:int}/events/{eventId:int}/tickets")]
    public async Task<IActionResult> GetTicketOwnerships(int userId, int eventId)
    {
        var ownerships = await _inventoryService.GetTicketOwnershipsAsync(userId, eventId);
        return Ok(ownerships.Select(o => o.ToDto()));
    }

    [HttpPost("events/tickets/remove")]
    public async Task<IActionResult> RemoveTicketOwnerships([FromBody] List<int> ownershipIds)
    {
        await _inventoryService.RemoveTicketOwnershipsAsync(ownershipIds);
        return Ok();
    }

    [HttpPost("ownedmovies")]
    public async Task<IActionResult> AddOwnedMovie([FromBody] AddOwnedMovieRequestBody body)
    {
        await _inventoryService.AddOwnedMovieAsync(body.UserId, body.MovieId);
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
            Event = t.Event == null ? null : (object)new
            {
                t.Event.Id,
                t.Event.Title,
                t.Event.Description,
                t.Event.Date,
                t.Event.Location,
                t.Event.TicketPrice,
                t.Event.PosterUrl,
                t.Event.Capacity,
            },
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

    [HttpGet("users/{userId:int}/equipment")]
    public async Task<IActionResult> GetOwnedEquipment(int userId)
    {
        var equipment = await _inventoryService.GetOwnedEquipmentAsync(userId);
        return Ok(equipment.Select(e => e.ToDto()));
    }

    [HttpPost("remove-equipment")]
    public async Task<IActionResult> RemoveOwnedEquipment([FromBody] RemoveEquipmentRequestBody body)
    {
        await _inventoryService.RemoveOwnedEquipmentAsync(body.UserId, body.EquipmentId);
        return Ok();
    }
}
