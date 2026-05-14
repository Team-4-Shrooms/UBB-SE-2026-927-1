using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.Mappings;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/reviews")]
public sealed class ReviewEndpointsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewEndpointsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetReviewsForMovie(int movieId)
    {
        var reviews = await _reviewService.GetReviewsForMovieAsync(movieId);
        return Ok(reviews.Select(review => review.ToDto(movieId)));
    }

    [HttpGet("movie/{movieId:int}/ratings")]
    public async Task<IActionResult> GetRawRatingsForMovie(int movieId)
    {
        return Ok(await _reviewService.GetStarRatingBucketsAsync(movieId));
    }

    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] AddReviewRequestBody body)
    {
        await _reviewService.PostReviewAsync(body.MovieId, body.UserId, body.StarRating, body.Comment);
        return Ok();
    }

    [HttpPost("counts")]
    public async Task<IActionResult> GetReviewCounts([FromBody] GetReviewCountsRequestBody request)
    {
        return Ok(await _reviewService.GetReviewCountsAsync(request.MovieIds));
    }
}
