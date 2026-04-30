using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebApi.Mappings;
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



    [HttpGet("{movieId:int}")]
    public async Task<IActionResult> GetMovieById(int movieId)
    {
        var movie = await _repository.GetMovieByIdAsync(movieId);

        if (movie == null) {
            return NoContent();
        }

        return Ok(movie.ToDto());
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTop10MoviesAsync([FromQuery] string? partialMovieName)
    {
        var movies = await _repository.SearchMoviesAsync(partialMovieName ?? string.Empty, 10);
        return Ok(movies.Select(movie => movie.ToDto()));
    }

    [HttpGet("{movieId:int}/owned/{userId:int}")]
    public async Task<IActionResult> UserOwnsMovie(int movieId, int userId)
    {
        return Ok(await _repository.UserOwnsMovieAsync(userId, movieId));
    }
}
