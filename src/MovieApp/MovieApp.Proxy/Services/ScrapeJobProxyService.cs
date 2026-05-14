using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Features.TrailerScraping;

namespace MovieApp.Proxy.Services
{
    public class ScrapeJobProxyService : IScrapeJobService
    {
        private readonly ApiClient _apiClient;

        public ScrapeJobProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<int> CreateJobAsync(ScrapeJob job)
        {
            return await _apiClient.PostAsync<ScrapeJob, int>("api/scrape-jobs", job);
        }

        public async Task<ScrapeJob?> GetJobByIdAsync(int jobId)
        {
            return await _apiClient.GetAsync<ScrapeJob>($"api/scrape-jobs/{jobId}");
        }

        public async Task UpdateJobAsync(ScrapeJob job)
        {
            await _apiClient.PutAsync($"api/scrape-jobs/{job.Id}", job);
        }

        public async Task AddLogEntryAsync(ScrapeJobLog log)
        {
            await _apiClient.PostAsync("api/scrape-jobs/logs", new { Level = log.Level, Message = log.Message, Timestamp = log.Timestamp, ScrapeJobId = log.ScrapeJob.Id });
        }

        public async Task<List<ScrapeJob>> GetAllJobsAsync()
        {
            var result = await _apiClient.GetAsync<List<ScrapeJob>>("api/scrape-jobs");
            return result ?? new List<ScrapeJob>();
        }

        public async Task<List<ScrapeJobLog>> GetLogsForJobAsync(int jobId)
        {
            var result = await _apiClient.GetAsync<List<ScrapeJobLog>>($"api/scrape-jobs/{jobId}/logs");
            return result ?? new List<ScrapeJobLog>();
        }

        public async Task<List<ScrapeJobLog>> GetAllLogsAsync()
        {
            var result = await _apiClient.GetAsync<List<ScrapeJobLog>>("api/scrape-jobs/logs");
            return result ?? new List<ScrapeJobLog>();
        }

        public async Task<MovieApp.Logic.Features.TrailerScraping.DashboardStatsModel> GetDashboardStatsAsync()
        {
            var result = await _apiClient.GetAsync<MovieApp.Logic.Features.TrailerScraping.DashboardStatsModel>("api/scrape-jobs/dashboard-stats");
            return result ?? new MovieApp.Logic.Features.TrailerScraping.DashboardStatsModel();
        }

        public async Task<List<Movie>> SearchMoviesByNameAsync(string partialName)
        {
            var result = await _apiClient.GetAsync<List<Movie>>($"api/scrape-jobs/search-movies?partialName={partialName}");
            return result ?? new List<Movie>();
        }

        public async Task<int?> FindMovieByTitleAsync(string title)
        {
            return await _apiClient.GetAsync<int?>($"api/scrape-jobs/movie-id?title={title}");
        }

        public async Task<bool> ReelExistsByVideoUrlAsync(string videoUrl)
        {
            return await _apiClient.GetAsync<bool>($"api/scrape-jobs/reel-exists?videoUrl={videoUrl}");
        }

        public async Task<int> InsertScrapedReelAsync(Reel reel)
        {
            return await _apiClient.PostAsync<Reel, int>("api/scrape-jobs/reels", reel);
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            var result = await _apiClient.GetAsync<List<Movie>>("api/scrape-jobs/movies");
            return result ?? new List<Movie>();
        }

        public async Task<List<Reel>> GetAllReelsAsync()
        {
            var result = await _apiClient.GetAsync<List<Reel>>("api/scrape-jobs/reels");
            return result ?? new List<Reel>();
        }
    }
}
