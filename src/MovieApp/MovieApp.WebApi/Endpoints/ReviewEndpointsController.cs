using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.DTO.WebAPI;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/reviews")]
public sealed class ReviewEndpointsController : ControllerBase
{
    private readonly ReviewRepository _repository;

    public ReviewEndpointsController(ReviewRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetReviewsForMovie(int movieId)
    {
        var reviews = await _repository.GetReviewsForMovieAsync(movieId);
        return Ok(reviews.Select(review => review.ToDto(movieId)));
    }

    [HttpPost("counts")]
    public async Task<IActionResult> GetReviewCounts([FromBody] GetReviewCountsRequestBody request)
    {
        return Ok(await _repository.GetReviewCountsAsync(request.MovieIds));
    }
}
