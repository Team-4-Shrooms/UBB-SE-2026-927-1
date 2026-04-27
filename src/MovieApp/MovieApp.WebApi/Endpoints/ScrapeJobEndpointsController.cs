using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/scrape-jobs")]
public sealed class ScrapeJobEndpointsController : ControllerBase
{
    private readonly ScrapeJobRepository _repository;

    public ScrapeJobEndpointsController(ScrapeJobRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateJobAsync([FromBody] ScrapeJob job)
    {
        return Ok(await _repository.CreateJobAsync(job));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateJobAsync([FromBody] ScrapeJob job)
    {
        await _repository.UpdateJobAsync(job);
        return Ok();
    }

    [HttpPost("logs")]
    public async Task<IActionResult> AddLogEntryAsync([FromBody] ScrapeJobLog log)
    {
        await _repository.AddLogEntryAsync(log);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobsAsync()
    {
        return Ok(await _repository.GetAllJobsAsync());
    }

    [HttpGet("{jobId:int}/logs")]
    public async Task<IActionResult> GetLogsForJobAsync(int jobId)
    {
        return Ok(await _repository.GetLogsForJobAsync(jobId));
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetAllLogsAsync()
    {
        return Ok(await _repository.GetAllLogsAsync());
    }

    [HttpGet("dashboard-stats")]
    public async Task<IActionResult> GetDashboardStatsAsync()
    {
        return Ok(await _repository.GetDashboardStatsAsync());
    }

    [HttpGet("search-movies")]
    public async Task<IActionResult> SearchMoviesByNameAsync([FromQuery] string partialName)
    {
        return Ok(await _repository.SearchMoviesByNameAsync(partialName));
    }

    [HttpGet("movie-id")]
    public async Task<IActionResult> FindMovieByTitleAsync([FromQuery] string title)
    {
        return Ok(await _repository.FindMovieByTitleAsync(title));
    }

    [HttpGet("reel-exists")]
    public async Task<IActionResult> ReelExistsByVideoUrlAsync([FromQuery] string videoUrl)
    {
        return Ok(await _repository.ReelExistsByVideoUrlAsync(videoUrl));
    }

    [HttpPost("reels")]
    public async Task<IActionResult> InsertScrapedReelAsync([FromBody] Reel reel)
    {
        return Ok(await _repository.InsertScrapedReelAsync(reel));
    }

    [HttpGet("movies")]
    public async Task<IActionResult> GetAllMoviesAsync()
    {
        return Ok(await _repository.GetAllMoviesAsync());
    }

    [HttpGet("reels")]
    public async Task<IActionResult> GetAllReelsAsync()
    {
        return Ok(await _repository.GetAllReelsAsync());
    }
}
