using Microsoft.AspNetCore.Mvc;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Mappings;
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
    public async Task<IActionResult> GetOwnedMovies(int userId)
    {
        var movies = await _repository.GetOwnedMoviesAsync(userId);
        return Ok(movies.Select(movie => movie.ToOwnedMovieDto(userId)));
    }
}
