using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
{
    public class ScrapeJobProxyRepository : IScrapeJobRepository
    {
        private readonly ApiClient _apiClient;

        public ScrapeJobProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<int> CreateJobAsync(ScrapeJob job)
        {
            var createdJob = await _apiClient.PostAsync<ScrapeJob>("api/scrape-jobs", job);
            return createdJob?.Id ?? 0;
        }

        public async Task UpdateJobAsync(ScrapeJob job)
        {
            await _apiClient.PutAsync($"api/scrape-jobs/{job.Id}", job);
        }

        public async Task AddLogEntryAsync(ScrapeJobLog log)
        {
            await _apiClient.PostAsync("api/scrape-jobs/logs", log);
        }

        public async Task<IList<ScrapeJob>> GetAllJobsAsync()
        {
            return await _apiClient.GetAllAsync<ScrapeJob>("api/scrape-jobs");
        }

        public async Task<IList<ScrapeJobLog>> GetLogsForJobAsync(int jobId)
        {
            return await _apiClient.GetAllAsync<ScrapeJobLog>($"api/scrape-jobs/{jobId}/logs");
        }

        public async Task<IList<ScrapeJobLog>> GetAllLogsAsync()
        {
            return await _apiClient.GetAllAsync<ScrapeJobLog>("api/scrape-jobs/logs");
        }

        public async Task<DashboardStatsModel> GetDashboardStatsAsync()
        {
            var results = await _apiClient.GetAllAsync<DashboardStatsModel>("api/scrape-jobs/dashboard-stats");
            return results.Count > 0 ? results[0] : new DashboardStatsModel();
        }

        public async Task<IList<Movie>> SearchMoviesByNameAsync(string partialName)
        {
            return await _apiClient.GetAllAsync<Movie>($"api/scrape-jobs/search-movies?partialName={partialName}");
        }

        public async Task<int?> FindMovieByTitleAsync(string title)
        {
            var results = await _apiClient.GetAllAsync<int?>($"api/scrape-jobs/movie-id?title={title}");
            return results.Count > 0 ? results[0] : null;
        }

        public async Task<bool> ReelExistsByVideoUrlAsync(string videoUrl)
        {
            var results = await _apiClient.GetAllAsync<bool>($"api/scrape-jobs/reel-exists?videoUrl={videoUrl}");
            return results.Count > 0 && results[0];
        }

        public async Task<int> InsertScrapedReelAsync(Reel reel)
        {
            var createdReel = await _apiClient.PostAsync<Reel>("api/scrape-jobs/reels", reel);
            return createdReel?.Id ?? 0;
        }

        public async Task<IList<Movie>> GetAllMoviesAsync()
        {
            return await _apiClient.GetAllAsync<Movie>("api/scrape-jobs/movies");
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            return await _apiClient.GetAllAsync<Reel>("api/scrape-jobs/reels");
        }
    }
}