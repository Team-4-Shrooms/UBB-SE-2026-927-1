using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Features.TrailerScraping;
using MovieApp.Logic.Interfaces;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.WebApi.Endpoints
{
    [Authorize]
    [ApiController]
    [Route("api/video-ingestion")]
    public class VideoIngestionEndpointsController : ControllerBase
    {
        private readonly IVideoIngestionService _ingestionService;
        private readonly IMovieRepository _movieRepository;

        public VideoIngestionEndpointsController(
            IVideoIngestionService ingestionService,
            IMovieRepository movieRepository)
        {
            _ingestionService = ingestionService;
            _movieRepository = movieRepository;
        }

        [HttpPost("run-scrape")]
        public async Task<IActionResult> RunScrape([FromBody] RunScrapeRequestBody body)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(body.MovieId);
            if (movie == null) return NotFound();
            var scrapeJob = await _ingestionService.RunScrapeJobAsync(movie, body.MaxResults);
                        return Accepted(new { JobId = scrapeJob.Id });
        }

        [HttpGet("jobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _ingestionService.GetAllJobsAsync();
            return Ok(jobs);
        }

        [HttpGet("jobs/{jobId:int}")]
        public async Task<IActionResult> GetJobStatus(int jobId)
        {
            var status = await _ingestionService.GetJobStatusAsync(jobId);

            if (status == null)
            {
                return NotFound($"Job with ID {jobId} was not found.");
            }

            return Ok(status);
        }

        [HttpPost("ingest-url")]
        public async Task<IActionResult> IngestUrl([FromBody] IngestUrlRequestBody body)
        {
            var result = await _ingestionService.IngestVideoFromUrlAsync(body.TrailerUrl, body.MovieId);
            return Ok(new { url = result });
        }

        [HttpGet("reels")]
        public async Task<IActionResult> GetAllReels()
        {
            var reels = await _ingestionService.GetAllReelsAsync();
            return Ok(reels);
        }
    }
}
