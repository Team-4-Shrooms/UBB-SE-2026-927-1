using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Features.TrailerScraping;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.ViewModels.TrailerScraping;
using MovieApp.WebApi.Mappings;

namespace MovieApp.Web.Controllers
{
    /// <summary>Stub controller for the Trailer Scraping feature.</summary>
    public sealed class TrailerScrapingController : Controller
    {
        /// <summary>Placeholder index page.</summary>

        private readonly IVideoIngestionService _ingestionService;
        private readonly IMovieService _movieService;
        private readonly ICurrentUserService _currentUserService;

        public TrailerScrapingController(
            IVideoIngestionService ingestionService,
            IMovieService movieService,
            ICurrentUserService currentUserService)
        {
            _ingestionService = ingestionService;
            _movieService = movieService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rawMovies = await _movieService.GetAllMoviesAsync();
            var rawJobs = await _ingestionService.GetAllJobsAsync();

            var viewModel = new TrailerScrapingIndexViewModel
            {
                AvailableMovies = rawMovies.Select(movie => movie.ToDto()),
                RecentJobs = rawJobs.Select(job => job.ToDto())
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Scrape(ScrapeForm form)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            int jobId = 0;

            if (!string.IsNullOrWhiteSpace(form.YouTubeUrl))
            {
                await _ingestionService.IngestVideoFromUrlAsync(form.YouTubeUrl, form.MovieId);
                TempData["SuccessMessage"] = "Video ingested successfully from URL!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var movie = await _movieService.GetMovieByIdAsync(form.MovieId);
                if (movie == null) return NotFound();

                var job = await _ingestionService.RunScrapeJobAsync(movie, form.MaxResults);

                jobId = job.Id;
            }

            TempData["NewJobId"] = jobId;
            TempData["SuccessMessage"] = "Scraping job started successfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Status(int jobId)
        {
            var job = await _ingestionService.GetJobStatusAsync(jobId);

            if (job == null) return NotFound();

            return Json(new
            {
                status = job.Status,
                logCount = job.Logs?.Count ?? 0
            });
        }
    }
}
