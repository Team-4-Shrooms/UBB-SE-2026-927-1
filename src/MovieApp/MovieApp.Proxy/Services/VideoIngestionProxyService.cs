using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.TrailerScraping;

namespace MovieApp.Proxy.Services
{
    public class VideoIngestionProxyService : IVideoIngestionService
    {
        private readonly ApiClient _apiClient;

        public VideoIngestionProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<string> IngestVideoFromUrlAsync(string trailerUrl, int movieId)
        {
            var result = await _apiClient.PostAsync<object, string>("api/ingestion/url", new { TrailerUrl = trailerUrl, MovieId = movieId });
            return result ?? string.Empty;
        }

        public async Task<ScrapeJob> RunScrapeJobAsync(Movie movie, int maxResults, Func<ScrapeJobLog, Task>? onLogEntry = null)
        {
            var result = await _apiClient.PostAsync<object, ScrapeJob>("api/ingestion/scrape", new { MovieId = movie.Id, MaxResults = maxResults });
            return result ?? new ScrapeJob();
        }

        public async Task<ScrapeJob?> GetJobStatusAsync(int jobId)
        {
            return await _apiClient.GetAsync<ScrapeJob>($"api/ingestion/job/{jobId}");
        }

        public async Task<IList<ScrapeJob>> GetAllJobsAsync()
        {
            var result = await _apiClient.GetAsync<List<ScrapeJob>>($"api/ingestion/jobs");
            return result ?? new List<ScrapeJob>();
        }
    }
}
