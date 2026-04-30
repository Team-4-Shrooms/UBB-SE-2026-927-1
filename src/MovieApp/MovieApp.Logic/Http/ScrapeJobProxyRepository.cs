using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Http
{
    public class ScrapeJobProxyRepository : IScrapeRepository
    {
        private readonly ApiClient _apiClient;

        public ScrapeJobProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<int> CreateJobAsync(ScrapeJob job)
        {
            int? createdJobId = await _apiClient.PostAsync<int>("api/scrape-jobs", job);
            return createdJobId ?? 0;
        }

        public async Task UpdateJobAsync(ScrapeJob job)
        {
            await _apiClient.PutAsync($"api/scrape-jobs/{job.Id}", job);
        }

        public async Task AddLogEntryAsync(ScrapeJobLog log)
        {
            await _apiClient.PostAsync("api/scrape-jobs/logs", (object)log);
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
            return await _apiClient.GetAsync<DashboardStatsModel>("api/scrape-jobs/dashboard-stats") ?? new DashboardStatsModel();
        }

        public async Task<IList<Movie>> SearchMoviesByNameAsync(string partialName)
        {
            return await _apiClient.GetAllAsync<Movie>($"api/scrape-jobs/search-movies?partialName={partialName}");
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
            int? createdReelId = await _apiClient.PostAsync<int>("api/scrape-jobs/reels", new
            {
                videoUrl = reel.VideoUrl,
                thumbnailUrl = reel.ThumbnailUrl,
                title = reel.Title,
                caption = reel.Caption,
                featureDurationSeconds = reel.FeatureDurationSeconds,
                cropDataJson = reel.CropDataJson,
                backgroundMusicId = reel.BackgroundMusicId,
                source = reel.Source,
                genre = reel.Genre,
                createdAt = reel.CreatedAt,
                lastEditedAt = reel.LastEditedAt,
                movieId = reel.Movie?.Id ?? 0,
                creatorUserId = reel.CreatorUser?.Id ?? 0,
            });
            return createdReelId ?? 0;
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
