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
            string endpoint = "api/video-ingestion/jobs";
            var result = await _apiClient.GetAsync<List<ScrapeJob>>(endpoint);
            return result ?? new List<ScrapeJob>();
        }

        public async Task<ScrapeJob?> GetJobStatusAsync(int jobId)
        {
            string endpoint = $"api/video-ingestion/jobs/{jobId}";
            return await _apiClient.GetAsync<ScrapeJob>(endpoint);
        }

        public async Task<ScrapeJob> RunScrapeJobAsync(Movie movie, int maxResults, Func<ScrapeJobLog, Task>? onLogEntry = null)
        {
            return await _apiClient.PostAsync<object, ScrapeJob>("api/video-ingestion/run-scrape", new
            {
                MovieId = movie.Id,
                MaxResults = maxResults
            });
        }

        public async Task<string> IngestVideoFromUrlAsync(string trailerUrl, int movieId)
        {
            var result = await _apiClient.PostAsync<object, JsonElement>("api/video-ingestion/ingest-url", new
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
