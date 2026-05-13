using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.TrailerScraping;
using System.Text.Json;

namespace MovieApp.Proxy.Services
{
    public class VideoIngestionProxyService : IVideoIngestionService
    {
        private readonly ApiClient _apiClient;

        public VideoIngestionProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IList<ScrapeJob>> GetAllJobsAsync()
        {
            // Calls the correct controller!
            var result = await _apiClient.GetAsync<List<ScrapeJob>>("api/video-ingestion/jobs");
            return result ?? new List<ScrapeJob>();
        }

        public async Task<ScrapeJob?> GetJobStatusAsync(int jobId)
        {
            // Calls the exact endpoint required by T9
            return await _apiClient.GetAsync<ScrapeJob>($"api/video-ingestion/jobs/{jobId}");
        }

        public async Task<ScrapeJob> RunScrapeJobAsync(Movie movie, int maxResults, Func<ScrapeJobLog, Task>? onLogEntry = null)
        {
            // Triggers the background worker on the server to actually download the videos!
            var response = await _apiClient.PostAsync<object, JsonElement>("api/video-ingestion/run-scrape", new
            {
                MovieId = movie.Id,
                MaxResults = maxResults
            });

            // The WebApi returns an object like { JobId = 5 }
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
            // Triggers the server to download the direct URL
            var result = await _apiClient.PostAsync<object, JsonElement>("api/video-ingestion/ingest-url", new
            {
                TrailerUrl = trailerUrl,
                MovieId = movieId
            });

            return result.GetProperty("url").GetString() ?? string.Empty;
        }
    }
}
