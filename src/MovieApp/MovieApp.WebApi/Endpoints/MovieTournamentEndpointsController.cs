using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/movie-tournament")]
public sealed class MovieTournamentEndpointsController : ControllerBase
{
    private readonly MovieTournamentRepository _repository;

    public MovieTournamentEndpointsController(MovieTournamentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("users/{userId:int}/pool-size")]
    public async Task<IActionResult> GetTournamentPoolSizeAsync(int userId)
    {
        return Ok(await _repository.GetTournamentPoolSizeAsync(userId));
    }

    [HttpGet("users/{userId:int}/pool")]
    public async Task<IActionResult> GetTournamentPoolAsync(int userId, [FromQuery] int poolSize)
    {
        var pool = await _repository.GetTournamentPoolAsync(userId, poolSize);
        return Ok(pool.Select(movie => movie.ToDto()));
    }

    [HttpPost("users/{userId:int}/movies/{movieId:int}/boost")]
    public async Task<IActionResult> BoostMovieScoreAsync(int userId, int movieId, [FromBody] BoostMovieScoreRequestBody request)
    {
        await _repository.BoostMovieScoreAsync(userId, movieId, request.ScoreBoost);
        return Ok();
    }
}
