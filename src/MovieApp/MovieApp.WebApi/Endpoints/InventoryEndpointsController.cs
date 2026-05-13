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
