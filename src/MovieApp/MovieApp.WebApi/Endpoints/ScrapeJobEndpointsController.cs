using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.DTO.WebAPI;
using MovieApp.DataLayer.Models;
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
    public async Task<IActionResult> CreateJobAsync([FromBody] ScrapeJobRequestBody job)
    {
        return Ok(await _repository.CreateJobAsync(job.ToModel()));
    }

    [HttpPut("{jobId:int}")]
    public async Task<IActionResult> UpdateJobAsync(int jobId, [FromBody] ScrapeJobRequestBody job)
    {
        ScrapeJob jobModel = job.ToModel();
        jobModel.Id = jobId;

        await _repository.UpdateJobAsync(jobModel);
        return Ok();
    }

    [HttpPost("logs")]
    public async Task<IActionResult> AddLogEntryAsync([FromBody] AddLogEntryRequestBody log)
    {
        await _repository.AddLogEntryAsync(log.ToModel());
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobsAsync()
    {
        var jobs = await _repository.GetAllJobsAsync();
        return Ok(jobs.Select(job => job.ToDto()));
    }

    [HttpGet("{jobId:int}/logs")]
    public async Task<IActionResult> GetLogsForJobAsync(int jobId)
    {
        var logs = await _repository.GetLogsForJobAsync(jobId);
        return Ok(logs.Select(log => log.ToDto()));
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetAllLogsAsync()
    {
        var logs = await _repository.GetAllLogsAsync();
        return Ok(logs.Select(log => log.ToDto()));
    }

    [HttpGet("dashboard-stats")]
    public async Task<IActionResult> GetDashboardStatsAsync()
    {
        return Ok((await _repository.GetDashboardStatsAsync()).ToDto());
    }

    [HttpGet("search-movies")]
    public async Task<IActionResult> SearchMoviesByNameAsync([FromQuery] string partialName)
    {
        var movies = await _repository.SearchMoviesByNameAsync(partialName);
        return Ok(movies.Select(movie => movie.ToDto()));
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
    public async Task<IActionResult> InsertScrapedReelAsync([FromBody] InsertReelRequestBody reel)
    {
        return Ok(await _repository.InsertScrapedReelAsync(reel.ToModel()));
    }

    [HttpGet("movies")]
    public async Task<IActionResult> GetAllMoviesAsync()
    {
        var movies = await _repository.GetAllMoviesAsync();
        return Ok(movies.Select(movie => movie.ToDto()));
    }

    [HttpGet("reels")]
    public async Task<IActionResult> GetAllReelsAsync()
    {
        var reels = await _repository.GetAllReelsAsync();
        return Ok(reels.Select(reel => reel.ToDto()));
    }
}
