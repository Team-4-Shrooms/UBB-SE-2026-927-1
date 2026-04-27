using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryEndpointsController : ControllerBase
{
    private readonly InventoryRepository _repository;

    public InventoryEndpointsController(InventoryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("users/{userId:int}/movies")]
    public IActionResult GetOwnedMovies(int userId)
    {
        return Ok(_repository.GetOwnedMovies(userId));
    }

    [HttpDelete("users/{userId:int}/movies/{movieId:int}")]
    public IActionResult RemoveOwnedMovie(int userId, int movieId)
    {
        _repository.RemoveOwnedMovie(userId, movieId);
        return Ok();
    }

    [HttpGet("users/{userId:int}/tickets")]
    public IActionResult GetOwnedTickets(int userId)
    {
        return Ok(_repository.GetOwnedTickets(userId));
    }

    [HttpDelete("users/{userId:int}/tickets/{eventId:int}")]
    public IActionResult RemoveOwnedTicket(int userId, int eventId)
    {
        _repository.RemoveOwnedTicket(userId, eventId);
        return Ok();
    }

    [HttpGet("users/{userId:int}/equipment")]
    public IActionResult GetOwnedEquipment(int userId)
    {
        return Ok(_repository.GetOwnedEquipment(userId));
    }
}
