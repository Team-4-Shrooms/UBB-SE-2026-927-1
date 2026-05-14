using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.TrailerScraping;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public class ScrapeJobService : IScrapeJobService
    {
        private readonly IScrapeJobRepository _scrapeJobRepository;

        public ScrapeJobService(IScrapeJobRepository scrapeJobRepository)
        {
            _scrapeJobRepository = scrapeJobRepository;
        }

        public async Task<int> CreateJobAsync(ScrapeJob job)
        {
            return await _scrapeJobRepository.CreateJobAsync(job);
        }

        public async Task<ScrapeJob?> GetJobByIdAsync(int jobId)
        {
            return await _scrapeJobRepository.GetJobByIdAsync(jobId);
        }

        public async Task UpdateJobAsync(ScrapeJob job)
        {
            await _scrapeJobRepository.UpdateJobAsync(job);
        }

        public async Task AddLogEntryAsync(ScrapeJobLog log)
        {
            await _scrapeJobRepository.AddLogEntryAsync(log);
        }

        public async Task<List<ScrapeJob>> GetAllJobsAsync()
        {
            var result = await _scrapeJobRepository.GetAllJobsAsync();
            return result.ToList();
        }

        public async Task<List<ScrapeJobLog>> GetLogsForJobAsync(int jobId)
        {
            var result = await _scrapeJobRepository.GetLogsForJobAsync(jobId);
            return result.ToList();
        }

        public async Task<List<ScrapeJobLog>> GetAllLogsAsync()
        {
            var result = await _scrapeJobRepository.GetAllLogsAsync();
            return result.ToList();
        }

        public async Task<Features.TrailerScraping.DashboardStatsModel> GetDashboardStatsAsync()
        {
            var stats = await _scrapeJobRepository.GetDashboardStatsAsync();
            return new Features.TrailerScraping.DashboardStatsModel
            {
                TotalMovies = stats.TotalMovies,
                TotalReels = stats.TotalReels,
                TotalJobs = stats.TotalJobs,
                RunningJobs = stats.RunningJobs,
                CompletedJobs = stats.CompletedJobs,
                FailedJobs = stats.FailedJobs,
            };
        }

        public async Task<List<Movie>> SearchMoviesByNameAsync(string partialName)
        {
            var result = await _scrapeJobRepository.SearchMoviesByNameAsync(partialName);
            return result.ToList();
        }

        public async Task<int?> FindMovieByTitleAsync(string title)
        {
            return await _scrapeJobRepository.FindMovieByTitleAsync(title);
        }

        public async Task<bool> ReelExistsByVideoUrlAsync(string videoUrl)
        {
            return await _scrapeJobRepository.ReelExistsByVideoUrlAsync(videoUrl);
        }

        public async Task<int> InsertScrapedReelAsync(Reel reel)
        {
            return await _scrapeJobRepository.InsertScrapedReelAsync(reel);
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            var result = await _scrapeJobRepository.GetAllMoviesAsync();
            return result.ToList();
        }

        public async Task<List<Reel>> GetAllReelsAsync()
        {
            var result = await _scrapeJobRepository.GetAllReelsAsync();
            return result.ToList();
        }
    }
}
