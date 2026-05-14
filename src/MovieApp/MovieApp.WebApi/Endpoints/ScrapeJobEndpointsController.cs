using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/scrape-jobs")]
public sealed class ScrapeJobEndpointsController : ControllerBase
{
    private readonly IScrapeJobService _scrapeJobService;
    private readonly IMovieService _movieService;
    private readonly IUserService _userService;

    public ScrapeJobEndpointsController(
        IScrapeJobService scrapeJobService,
        IMovieService movieService,
        IUserService userService)
    {
        _scrapeJobService = scrapeJobService;
        _movieService = movieService;
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateJobAsync([FromBody] ScrapeJobRequestBody job)
    {
        var jobId = await _scrapeJobService.CreateJobAsync(job.ToModel());
        return Ok(jobId);
    }

    [HttpPut("{jobId:int}")]
    public async Task<IActionResult> UpdateJobAsync(int jobId, [FromBody] ScrapeJobRequestBody job)
    {
        ScrapeJob jobModel = job.ToModel();
        jobModel.Id = jobId;

        await _scrapeJobService.UpdateJobAsync(jobModel);
        return Ok();
    }

    [HttpPost("logs")]
    public async Task<IActionResult> AddLogEntryAsync([FromBody] AddLogEntryRequestBody log)
    {
        ScrapeJob? scrapeJob = await _scrapeJobService.GetJobByIdAsync(log.ScrapeJobId);
        if (scrapeJob == null)
        {
            return NotFound($"Scrape job {log.ScrapeJobId} not found.");
        }

        ScrapeJobLog logModel = log.ToModel();
        logModel.ScrapeJob = scrapeJob;

        await _scrapeJobService.AddLogEntryAsync(logModel);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobsAsync()
    {
        var jobs = await _scrapeJobService.GetAllJobsAsync();
        return Ok(jobs.Select(job => job.ToDto()));
    }

    [HttpGet("{jobId:int}/logs")]
    public async Task<IActionResult> GetLogsForJobAsync(int jobId)
    {
        var logs = await _scrapeJobService.GetLogsForJobAsync(jobId);
        return Ok(logs.Select(log => log.ToDto()));
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetAllLogsAsync()
    {
        var logs = await _scrapeJobService.GetAllLogsAsync();
        return Ok(logs.Select(log => log.ToDto()));
    }

    [HttpGet("dashboard-stats")]
    public async Task<IActionResult> GetDashboardStatsAsync()
    {
        return Ok((await _scrapeJobService.GetDashboardStatsAsync()).ToDto());
    }

    [HttpGet("search-movies")]
    public async Task<IActionResult> SearchMoviesByNameAsync([FromQuery] string partialName)
    {
        var movies = await _scrapeJobService.SearchMoviesByNameAsync(partialName);
        return Ok(movies.Select(movie => movie.ToDto()));
    }

    [HttpGet("movie-id")]
    public async Task<IActionResult> FindMovieByTitleAsync([FromQuery] string title)
    {
        return Ok(await _scrapeJobService.FindMovieByTitleAsync(title));
    }

    [HttpGet("reel-exists")]
    public async Task<IActionResult> ReelExistsByVideoUrlAsync([FromQuery] string videoUrl)
    {
        return Ok(await _scrapeJobService.ReelExistsByVideoUrlAsync(videoUrl));
    }

    [HttpPost("reels")]
    public async Task<IActionResult> InsertScrapedReelAsync([FromBody] InsertReelRequestBody reel)
    {
        Movie? movie = await _movieService.GetMovieByIdAsync(reel.MovieId);
        if (movie == null) return NotFound($"Movie {reel.MovieId} not found.");

        User? creatorUser = await _userService.GetUserByIdAsync(reel.CreatorUserId);
        if (creatorUser == null) return NotFound($"User {reel.CreatorUserId} not found.");

        Reel reelModel = reel.ToModel();
        reelModel.Movie = movie;
        reelModel.CreatorUser = creatorUser;

        return Ok(await _scrapeJobService.InsertScrapedReelAsync(reelModel));
    }

    [HttpGet("movies")]
    public async Task<IActionResult> GetAllMoviesAsync()
    {
        var movies = await _scrapeJobService.GetAllMoviesAsync();
        return Ok(movies.Select(movie => movie.ToDto()));
    }

    [HttpGet("reels")]
    public async Task<IActionResult> GetAllReelsAsync()
    {
        var reels = await _scrapeJobService.GetAllReelsAsync();
        return Ok(reels.Select(reel => reel.ToDto()));
    }
}
