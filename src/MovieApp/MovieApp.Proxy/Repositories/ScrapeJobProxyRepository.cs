using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class ScrapeJobProxyRepository : IScrapeRepository
    {
        private readonly ApiClient _apiClient;

        public ScrapeJobProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<int> CreateJobAsync(ScrapeJob job)
        {
            var result = await _apiClient.PostAsync<object, int>("api/scrape-jobs", new
            {
                SearchQuery = job.SearchQuery,
                MaxResults = job.MaxResults,
                Status = job.Status,
                MoviesFound = job.MoviesFound,
                ReelsCreated = job.ReelsCreated,
                StartedAt = job.StartedAt,
                CompletedAt = job.CompletedAt,
                ErrorMessage = job.ErrorMessage,
            });
            return result;
        }

        public async Task UpdateJobAsync(ScrapeJob job)
        {
            await _apiClient.PutAsync($"api/scrape-jobs/{job.Id}", new
            {
                SearchQuery = job.SearchQuery,
                MaxResults = job.MaxResults,
                Status = job.Status,
                MoviesFound = job.MoviesFound,
                ReelsCreated = job.ReelsCreated,
                StartedAt = job.StartedAt,
                CompletedAt = job.CompletedAt,
                ErrorMessage = job.ErrorMessage,
            });
        }

        public async Task AddLogEntryAsync(ScrapeJobLog log)
        {
            await _apiClient.PostAsync("api/scrape-jobs/logs", new
            {
                Level = log.Level,
                Message = log.Message,
                Timestamp = log.Timestamp,
                ScrapeJobId = log.ScrapeJob?.Id ?? 0,
            });
        }

        public async Task<IList<ScrapeJob>> GetAllJobsAsync()
        {
            var result = await _apiClient.GetAsync<List<ScrapeJob>>("api/scrape-jobs");
            return result ?? new List<ScrapeJob>();
        }

        public Task<ScrapeJob?> GetJobByIdAsync(int jobId) => Task.FromResult<ScrapeJob?>(null);

        public async Task<IList<ScrapeJobLog>> GetLogsForJobAsync(int jobId)
        {
            var result = await _apiClient.GetAsync<List<ScrapeJobLog>>($"api/scrape-jobs/{jobId}/logs");
            return result ?? new List<ScrapeJobLog>();
        }

        public async Task<IList<ScrapeJobLog>> GetAllLogsAsync()
        {
            var result = await _apiClient.GetAsync<List<ScrapeJobLog>>("api/scrape-jobs/logs");
            return result ?? new List<ScrapeJobLog>();
        }

        public async Task<DashboardStatsModel> GetDashboardStatsAsync()
        {
            var result = await _apiClient.GetAsync<DashboardStatsModel>("api/scrape-jobs/dashboard-stats");
            return result ?? new DashboardStatsModel();
        }

        public async Task<IList<Movie>> SearchMoviesByNameAsync(string partialName)
        {
            var result = await _apiClient.GetAsync<List<Movie>>($"api/scrape-jobs/search-movies?partialName={Uri.EscapeDataString(partialName)}");
            return result ?? new List<Movie>();
        }

        public async Task<int?> FindMovieByTitleAsync(string title)
        {
            return await _apiClient.GetAsync<int?>($"api/scrape-jobs/movie-id?title={Uri.EscapeDataString(title)}");
        }

        public async Task<bool> ReelExistsByVideoUrlAsync(string videoUrl)
        {
            return await _apiClient.GetAsync<bool>($"api/scrape-jobs/reel-exists?videoUrl={Uri.EscapeDataString(videoUrl)}");
        }

        public async Task<int> InsertScrapedReelAsync(Reel reel)
        {
            var result = await _apiClient.PostAsync<object, int>("api/scrape-jobs/reels", new
            {
                VideoUrl = reel.VideoUrl,
                ThumbnailUrl = reel.ThumbnailUrl,
                Title = reel.Title,
                Caption = reel.Caption,
                FeatureDurationSeconds = reel.FeatureDurationSeconds,
                CropDataJson = reel.CropDataJson,
                BackgroundMusicId = reel.BackgroundMusicId,
                Source = reel.Source,
                Genre = reel.Genre,
                CreatedAt = reel.CreatedAt,
                LastEditedAt = reel.LastEditedAt,
                MovieId = reel.Movie?.Id ?? 0,
                CreatorUserId = reel.CreatorUser?.Id ?? 0,
            });
            return result;
        }

        public async Task<IList<Movie>> GetAllMoviesAsync()
        {
            var result = await _apiClient.GetAsync<List<Movie>>("api/scrape-jobs/movies");
            return result ?? new List<Movie>();
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            var result = await _apiClient.GetAsync<List<Reel>>("api/scrape-jobs/reels");
            return result ?? new List<Reel>();
        }
    }
}
