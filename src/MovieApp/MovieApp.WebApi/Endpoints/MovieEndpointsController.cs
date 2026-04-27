using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/movies")]
public sealed class MovieEndpointsController : ControllerBase
{
    private readonly MovieRepository _repository;

    public MovieEndpointsController(MovieRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult GetAllMovies()
    {
        return Ok(_repository.GetAllMovies());
    }

    [HttpGet("{movieId:int}")]
    public IActionResult GetMovieById(int movieId)
    {
        return Ok(_repository.GetMovieById(movieId));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTop10MoviesAsync([FromQuery] string? partialMovieName)
    {
        return Ok(await _repository.SearchTop10MoviesAsync(partialMovieName ?? string.Empty));
    }

    [HttpGet("{movieId:int}/owned/{userId:int}")]
    public IActionResult UserOwnsMovie(int movieId, int userId)
    {
        return Ok(_repository.UserOwnsMovie(userId, movieId));
    }

    [HttpPost("{movieId:int}/purchase")]
    public IActionResult PurchaseMovie(int movieId, [FromBody] PurchaseMovieRequest request)
    {
        try
        {
            _repository.PurchaseMovie(request.UserId, movieId, request.FinalPrice);
            return Ok();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    public sealed record PurchaseMovieRequest(int UserId, decimal FinalPrice);
}
