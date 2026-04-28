using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.DTO.WebAPI;
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
        return Ok(_repository.GetAllMovies().Select(movie => movie.ToDto()));
    }

    [HttpGet("{movieId:int}")]
    public IActionResult GetMovieById(int movieId)
    {
        MovieDto? movie = _repository.GetMovieById(movieId)?.ToDto();
        return Ok(movie);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTop10MoviesAsync([FromQuery] string? partialMovieName)
    {
        var movies = await _repository.SearchTop10MoviesAsync(partialMovieName ?? string.Empty);
        return Ok(movies.Select(movie => movie.ToDto()));
    }

    [HttpGet("{movieId:int}/owned/{userId:int}")]
    public IActionResult UserOwnsMovie(int movieId, int userId)
    {
        return Ok(_repository.UserOwnsMovie(userId, movieId));
    }

    [HttpPost("{movieId:int}/purchase")]
    public IActionResult PurchaseMovie(int movieId, [FromBody] PurchaseMovieRequestBody request)
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
}
