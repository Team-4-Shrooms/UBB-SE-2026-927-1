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
    public IActionResult GetReviewsForMovie(int movieId)
    {
        return Ok(_repository.GetReviewsForMovie(movieId).Select(review => review.ToDto(movieId)));
    }

    [HttpPost]
    public IActionResult AddReview([FromBody] AddReviewRequestBody request)
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
    public IActionResult GetReviewCounts([FromBody] GetReviewCountsRequestBody request)
    {
        return Ok(_repository.GetReviewCounts(request.MovieIds));
    }

    [HttpGet("movie/{movieId:int}/buckets")]
    public IActionResult GetStarRatingBuckets(int movieId)
    {
        return Ok(_repository.GetStarRatingBuckets(movieId));
    }
}
