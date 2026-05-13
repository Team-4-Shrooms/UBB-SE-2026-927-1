using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.TrailerScraping;
using System.Text.Json;

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
            var result = await _apiClient.GetAsync<List<ScrapeJob>>("api/video-ingestion/jobs");
            return result ?? new List<ScrapeJob>();
        }

        public async Task<ScrapeJob?> GetJobStatusAsync(int jobId)
        {
            return await _apiClient.GetAsync<ScrapeJob>($"api/video-ingestion/jobs/{jobId}");
        }

        public async Task<ScrapeJob> RunScrapeJobAsync(Movie movie, int maxResults, Func<ScrapeJobLog, Task>? onLogEntry = null)
        {
            var response = await _apiClient.PostAsync<object, JsonElement>("api/video-ingestion/run-scrape", new
            {
                MovieId = movie.Id,
                MaxResults = maxResults
            });

            int jobId = response.GetProperty("jobId").GetInt32();

            return new ScrapeJob
            {
                Id = jobId,
                SearchQuery = movie.Title,
                MaxResults = maxResults,
                Status = "pending",
                StartedAt = DateTime.UtcNow,
            };
        }

        public async Task<string> IngestVideoFromUrlAsync(string trailerUrl, int movieId)
        {
            var result = await _apiClient.PostAsync<object, JsonElement>("api/video-ingestion/ingest-url", new
            {
                TrailerUrl = trailerUrl,
                MovieId = movieId
            });

            return result.GetProperty("url").GetString() ?? string.Empty;
        }
    }
}
