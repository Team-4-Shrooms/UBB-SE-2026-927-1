using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MovieApp.Tests.Repositories
{
    public class ScrapeJobRepositoryTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        private static async Task<(User user, Movie movie)> SeedUserAndMovie(AppDbContext context)
        {
            User user = new User
            {
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = "hash",
                Balance = 0m,
            };

            Movie movie = new Movie
            {
                Title = "Test Movie",
                Description = "desc",
                Rating = 8m,
                Price = 10m,
                PrimaryGenre = "Action",
                ReleaseYear = 2020,
                Synopsis = "synopsis",
            };

            context.Users.Add(user);
            context.Movies.Add(movie);
            await context.SaveChangesAsync();

            return (user, movie);
        }

        [Fact]
        public async Task CreateJobAsync_validJob_insertsAndReturnsId()
        {
            await using AppDbContext context = CreateContext(nameof(CreateJobAsync_validJob_insertsAndReturnsId));

            ScrapeJob job = new ScrapeJob
            {
                SearchQuery = "action movies",
                MaxResults = 10,
                Status = "pending",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            int id = await repository.CreateJobAsync(job);

            Assert.True(id > 0);
            Assert.Equal(1, await context.ScrapeJobs.CountAsync());
        }

        [Fact]
        public async Task UpdateJobAsync_existingJob_updatesStatus()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateJobAsync_existingJob_updatesStatus));

            ScrapeJob job = new ScrapeJob
            {
                SearchQuery = "action movies",
                MaxResults = 10,
                Status = "pending",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(job);
            await context.SaveChangesAsync();

            job.Status = "completed";
            job.MoviesFound = 5;
            job.CompletedAt = DateTime.UtcNow;

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            await repository.UpdateJobAsync(job);

            ScrapeJob? updated = await context.ScrapeJobs.FindAsync(job.Id);
            Assert.Equal("completed", updated!.Status);
            Assert.Equal(5, updated.MoviesFound);
        }

        [Fact]
        public async Task UpdateJobAsync_nonExistentJob_doesNotThrow()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateJobAsync_nonExistentJob_doesNotThrow));

            ScrapeJob job = new ScrapeJob
            {
                Id = 999,
                SearchQuery = "test",
                MaxResults = 5,
                Status = "completed",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            await repository.UpdateJobAsync(job);

            Assert.Equal(0, await context.ScrapeJobs.CountAsync());
        }

        [Fact]
        public async Task AddLogEntryAsync_validLog_insertsLog()
        {
            await using AppDbContext context = CreateContext(nameof(AddLogEntryAsync_validLog_insertsLog));

            ScrapeJob job = new ScrapeJob
            {
                SearchQuery = "test",
                MaxResults = 5,
                Status = "running",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(job);
            await context.SaveChangesAsync();

            ScrapeJobLog log = new ScrapeJobLog
            {
                ScrapeJob = job,
                Level = "Info",
                Message = "Starting scrape",
                Timestamp = DateTime.UtcNow,
            };

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            await repository.AddLogEntryAsync(log);

            Assert.Equal(1, await context.ScrapeJobLogs.CountAsync());
        }

        [Fact]
        public async Task GetAllJobsAsync_jobsExist_returnsAllJobsOrderedByDateDescending()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllJobsAsync_jobsExist_returnsAllJobsOrderedByDateDescending));

            context.ScrapeJobs.AddRange(
                new ScrapeJob { SearchQuery = "old", MaxResults = 5, Status = "completed", MoviesFound = 0, ReelsCreated = 0, StartedAt = DateTime.UtcNow.AddDays(-2) },
                new ScrapeJob { SearchQuery = "new", MaxResults = 5, Status = "pending", MoviesFound = 0, ReelsCreated = 0, StartedAt = DateTime.UtcNow });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<ScrapeJob> result = await repository.GetAllJobsAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("new", result[0].SearchQuery);
        }

        [Fact]
        public async Task GetLogsForJobAsync_logsExist_returnsLogsForJob()
        {
            await using AppDbContext context = CreateContext(nameof(GetLogsForJobAsync_logsExist_returnsLogsForJob));

            ScrapeJob job = new ScrapeJob
            {
                SearchQuery = "test",
                MaxResults = 5,
                Status = "running",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(job);
            await context.SaveChangesAsync();

            context.ScrapeJobLogs.Add(new ScrapeJobLog
            {
                ScrapeJob = job,
                Level = "Info",
                Message = "Log message",
                Timestamp = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<ScrapeJobLog> result = await repository.GetLogsForJobAsync(job.Id);

            Assert.Single(result);
        }

        [Fact]
        public async Task ReelExistsByVideoUrlAsync_reelExists_returnsTrue()
        {
            await using AppDbContext context = CreateContext(nameof(ReelExistsByVideoUrlAsync_reelExists_returnsTrue));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.Reels.Add(new Reel
            {
                VideoUrl = "http://exists.url",
                ThumbnailUrl = "t",
                Title = "r",
                Caption = "c",
                FeatureDurationSeconds = 10m,
                Source = "scraped",
                CreatedAt = DateTime.UtcNow,
                Movie = movie,
                CreatorUser = user,
            });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            bool result = await repository.ReelExistsByVideoUrlAsync("http://exists.url");

            Assert.True(result);
        }

        [Fact]
        public async Task ReelExistsByVideoUrlAsync_reelDoesNotExist_returnsFalse()
        {
            await using AppDbContext context = CreateContext(nameof(ReelExistsByVideoUrlAsync_reelDoesNotExist_returnsFalse));

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            bool result = await repository.ReelExistsByVideoUrlAsync("http://notexists.url");

            Assert.False(result);
        }

        [Fact]
        public async Task FindMovieByTitleAsync_movieExists_returnsId()
        {
            await using AppDbContext context = CreateContext(nameof(FindMovieByTitleAsync_movieExists_returnsId));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            int? result = await repository.FindMovieByTitleAsync("Test Movie");

            Assert.NotNull(result);
            Assert.Equal(movie.Id, result);
        }

        [Fact]
        public async Task FindMovieByTitleAsync_movieDoesNotExist_returnsNull()
        {
            await using AppDbContext context = CreateContext(nameof(FindMovieByTitleAsync_movieDoesNotExist_returnsNull));

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            int? result = await repository.FindMovieByTitleAsync("Nonexistent Movie");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetDashboardStatsAsync_withData_returnsCorrectStats()
        {
            await using AppDbContext context = CreateContext(nameof(GetDashboardStatsAsync_withData_returnsCorrectStats));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.Reels.Add(new Reel
            {
                VideoUrl = "http://v.url",
                ThumbnailUrl = "t",
                Title = "r",
                Caption = "c",
                FeatureDurationSeconds = 10m,
                Source = "scraped",
                CreatedAt = DateTime.UtcNow,
                Movie = movie,
                CreatorUser = user,
            });

            context.ScrapeJobs.Add(new ScrapeJob
            {
                SearchQuery = "q",
                MaxResults = 5,
                Status = "completed",
                MoviesFound = 1,
                ReelsCreated = 1,
                StartedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            DashboardStatsModel result = await repository.GetDashboardStatsAsync();

            Assert.Equal(1, result.TotalMovies);
            Assert.Equal(1, result.TotalReels);
            Assert.Equal(1, result.TotalJobs);
            Assert.Equal(1, result.CompletedJobs);
            Assert.Equal(0, result.RunningJobs);
            Assert.Equal(0, result.FailedJobs);
        }
    }
}
