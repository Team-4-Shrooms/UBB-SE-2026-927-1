using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.DataLayer.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IScrapeJobRepository"/>.
    /// </summary>
    public class ScrapeJobRepository : IScrapeRepository
    {
        private const int MaxLogsToRetrieve = 200;
        private const int MaxMoviesToSearch = 20;

        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;

        public ScrapeJobRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<int> CreateJobAsync(ScrapeJob job)
        {
            _context.ScrapeJobs.Add(job);
            await _context.SaveChangesAsync();
            return job.Id;
        }

        /// <inheritdoc />
        public async Task UpdateJobAsync(ScrapeJob job)
        {
            ScrapeJob existing = await _context.ScrapeJobs.FindAsync(job.Id)
                ?? throw new KeyNotFoundException($"ScrapeJob {job.Id} not found.");

            existing.Status = job.Status;
            existing.MoviesFound = job.MoviesFound;
            existing.ReelsCreated = job.ReelsCreated;
            existing.CompletedAt = job.CompletedAt;
            existing.ErrorMessage = job.ErrorMessage;

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task AddLogEntryAsync(ScrapeJobLog log)
        {
            _context.ScrapeJobLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IList<ScrapeJob>> GetAllJobsAsync()
        {
            return await _context.ScrapeJobs
                .Include(job => job.Logs)
                .OrderByDescending(job => job.StartedAt)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IList<ScrapeJobLog>> GetLogsForJobAsync(int jobId)
        {
            return await _context.ScrapeJobLogs
                .Include(log => log.ScrapeJob)
                .Where(log => log.ScrapeJob.Id == jobId)
                .OrderBy(log => log.Timestamp)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IList<ScrapeJobLog>> GetAllLogsAsync()
        {
            return await _context.ScrapeJobLogs
                .Include(log => log.ScrapeJob)
                .OrderByDescending(log => log.Timestamp)
                .Take(MaxLogsToRetrieve)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<DashboardStatsModel> GetDashboardStatsAsync()
        {
            int totalMovies = await _context.Movies.CountAsync();
            int totalReels = await _context.Reels.CountAsync();
            int totalJobs = await _context.ScrapeJobs.CountAsync();
            int runningJobs = await _context.ScrapeJobs.CountAsync(job => job.Status == "running");
            int completedJobs = await _context.ScrapeJobs.CountAsync(job => job.Status == "completed");
            int failedJobs = await _context.ScrapeJobs.CountAsync(job => job.Status == "failed");

            return new DashboardStatsModel
            {
                TotalMovies = totalMovies,
                TotalReels = totalReels,
                TotalJobs = totalJobs,
                RunningJobs = runningJobs,
                CompletedJobs = completedJobs,
                FailedJobs = failedJobs,
            };
        }

        /// <inheritdoc />
        public async Task<IList<Movie>> SearchMoviesByNameAsync(string partialName)
        {
            return await _context.Movies
                .Where(movie => movie.Title.Contains(partialName))
                .OrderBy(movie => movie.Title)
                .Take(MaxMoviesToSearch)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<int?> FindMovieByTitleAsync(string title)
        {
            Movie? movie = await _context.Movies
                .FirstOrDefaultAsync(currentMovie => currentMovie.Title == title);

            return movie?.Id;
        }

        /// <inheritdoc />
        public async Task<bool> ReelExistsByVideoUrlAsync(string videoUrl)
        {
            return await _context.Reels
                .AnyAsync(reel => reel.VideoUrl == videoUrl);
        }

        /// <inheritdoc />
        public async Task<int> InsertScrapedReelAsync(Reel reel)
        {
            _context.Reels.Add(reel);
            await _context.SaveChangesAsync();
            return reel.Id;
        }

        /// <inheritdoc />
        public async Task<IList<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies
                .OrderBy(movie => movie.Title)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            return await _context.Reels
                .Include(reel => reel.Movie)
                .Include(reel => reel.CreatorUser)
                .OrderByDescending(reel => reel.CreatedAt)
                .ToListAsync();
        }
    }
}

