using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/reviews")]
public sealed class ReviewEndpointsController : ControllerBase
{
    private readonly ReviewRepository _repository;
    private readonly IMovieAppDbContext _context;

    public ReviewEndpointsController(ReviewRepository repository, IMovieAppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetReviewsForMovie(int movieId)
    {
        var reviews = await _repository.GetReviewsForMovieAsync(movieId);
        return Ok(reviews.Select(review => review.ToDto(movieId)));
    }

    [HttpGet("movie/{movieId:int}/ratings")]
    public async Task<IActionResult> GetRawRatingsForMovie(int movieId)
    {
        return Ok(await _repository.GetRawRatingsForMovieAsync(movieId));
    }

    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] AddReviewRequestBody body)
    {
        var user = await _context.Users.FindAsync(body.UserId)
            ?? throw new InvalidOperationException($"User {body.UserId} not found.");
        var movie = await _context.Movies.FindAsync(body.MovieId)
            ?? throw new InvalidOperationException($"Movie {body.MovieId} not found.");
        var review = new MovieReview
        {
            StarRating = body.StarRating,
            Comment = body.Comment,
            CreatedAt = DateTime.UtcNow,
            User = user,
            Movie = movie,
        };
        await _repository.AddReviewAsync(review);
        await _repository.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("counts")]
    public async Task<IActionResult> GetReviewCounts([FromBody] GetReviewCountsRequestBody request)
    {
        return Ok(await _repository.GetReviewCountsAsync(request.MovieIds));
    }
}
