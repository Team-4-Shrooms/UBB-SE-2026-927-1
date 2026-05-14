using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
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

        private const string JobProperty = "jobId";
        private const string PendingStatus = "pending";
        private const string UrlProperty = "url";

        public VideoIngestionProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IList<ScrapeJob>> GetAllJobsAsync()
        {
            string key = "api/video-ingestion/jobs";
            var result = await _apiClient.GetAsync<List<ScrapeJob>>(key);
            return result ?? new List<ScrapeJob>();
        }

        public async Task<ScrapeJob?> GetJobStatusAsync(int jobId)
        {
            string key = $"api/video-ingestion/jobs/{jobId}";
            return await _apiClient.GetAsync<ScrapeJob>(key);
        }

        public async Task<ScrapeJob> RunScrapeJobAsync(Movie movie, int maxResults, Func<ScrapeJobLog, Task>? onLogEntry = null)
        {
            string key = "api/video-ingestion/run-scrape";
            var response = await _apiClient.PostAsync<object, JsonElement>(key, new
            {
                MovieId = movie.Id,
                MaxResults = maxResults
            });

            int jobId = response.GetProperty(JobProperty).GetInt32();

            return new ScrapeJob
            {
                Id = jobId,
                SearchQuery = movie.Title,
                MaxResults = maxResults,
                Status = PendingStatus,
                StartedAt = DateTime.UtcNow,
            };
        }

        public async Task<string> IngestVideoFromUrlAsync(string trailerUrl, int movieId)
        {
            string key = "api/video-ingestion/ingest-url";

            var result = await _apiClient.PostAsync<object, JsonElement>(key, new
            {
                TrailerUrl = trailerUrl,
                MovieId = movieId
            });

            if (result.TryGetProperty("url", out var urlElement))
            {
                return urlElement.GetString() ?? string.Empty;
            }

            return string.Empty;
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            try
            {
                string key = "api/video-ingestion/reels";
                var reels = await _apiClient.GetAsync<List<Reel>>(key);
                return reels ?? new List<Reel>();
            }
            catch
            {
                return new List<Reel>();
            }
        }
    }
}
