using Microsoft.AspNetCore.Mvc;
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
    public IActionResult GetReviewsForMovie(int movieId)
    {
        return Ok(_repository.GetReviewsForMovie(movieId));
    }

    [HttpPost]
    public IActionResult AddReview([FromBody] AddReviewRequest request)
    {
        try
        {
            _repository.AddReview(request.MovieId, request.UserId, request.StarRating, request.Comment);
            return Ok();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("movie/{movieId:int}/count")]
    public IActionResult GetReviewCount(int movieId)
    {
        return Ok(_repository.GetReviewCount(movieId));
    }

    [HttpPost("counts")]
    public IActionResult GetReviewCounts([FromBody] MovieIdsRequest request)
    {
        return Ok(_repository.GetReviewCounts(request.MovieIds));
    }

    [HttpGet("movie/{movieId:int}/buckets")]
    public IActionResult GetStarRatingBuckets(int movieId)
    {
        return Ok(_repository.GetStarRatingBuckets(movieId));
    }

    public sealed record AddReviewRequest(int MovieId, int UserId, int StarRating, string? Comment);
    public sealed record MovieIdsRequest(IEnumerable<int> MovieIds);
}
