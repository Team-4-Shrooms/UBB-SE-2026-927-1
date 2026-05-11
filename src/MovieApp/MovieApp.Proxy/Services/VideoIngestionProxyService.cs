using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.TrailerScraping;

namespace MovieApp.Proxy.Services
{
    /// <summary>
    /// Proxy implementation of IVideoIngestionService.
    /// Read-only operations (GetAllJobsAsync, GetJobStatusAsync) call the scrape-job WebApi.
    /// RunScrapeJobAsync and IngestVideoFromUrlAsync create scrape-job / reel records on the server;
    /// the actual YouTube download is done server-side by the real VideoIngestionService.
    /// </summary>
    public class VideoIngestionProxyService : IVideoIngestionService
    {
        private readonly ApiClient _apiClient;

        public VideoIngestionProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IList<ScrapeJob>> GetAllJobsAsync()
        {
            var result = await _apiClient.GetAsync<List<ScrapeJob>>("api/scrape-jobs");
            return result ?? new List<ScrapeJob>();
        }

        public async Task<ScrapeJob?> GetJobStatusAsync(int jobId)
        {
            // There is no single-job-by-id endpoint; fall back to filtering the list.
            var all = await GetAllJobsAsync();
            return all.FirstOrDefault(j => j.Id == jobId);
        }

        public async Task<ScrapeJob> RunScrapeJobAsync(Movie movie, int maxResults,
            Func<ScrapeJobLog, Task>? onLogEntry = null)
        {
            // Create a scrape-job record on the server (the server-side service handles
            // the actual YouTube scraping when its own VideoIngestionService runs).
            int jobId = await _apiClient.PostAsync<object, int>("api/scrape-jobs", new
            {
                SearchQuery = movie.Title,
                MaxResults = maxResults,
                Status = "running",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            });

            return new ScrapeJob
            {
                Id = jobId,
                SearchQuery = movie.Title,
                MaxResults = maxResults,
                Status = "running",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };
        }

        public async Task<string> IngestVideoFromUrlAsync(string trailerUrl, int movieId)
        {
            // Check if a reel for this URL already exists.
            bool exists = await _apiClient.GetAsync<bool>(
                $"api/scrape-jobs/reel-exists?videoUrl={Uri.EscapeDataString(trailerUrl)}");
            if (exists) return string.Empty;

            // Insert a reel record that points to the remote trailer URL directly.
            int reelId = await _apiClient.PostAsync<object, int>("api/scrape-jobs/reels", new
            {
                VideoUrl = trailerUrl,
                ThumbnailUrl = string.Empty,
                Title = "Scraped Trailer",
                Caption = string.Empty,
                Source = "scraped",
                Genre = string.Empty,
                FeatureDurationSeconds = 0m,
                CropDataJson = "{}",
                CreatedAt = DateTime.UtcNow,
                MovieId = movieId,
                CreatorUserId = 1,
            });

            return reelId.ToString();
        }
    }
}
