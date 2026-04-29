using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MovieApp.Tests.Repositories
{
    public class ScrapeJobRepositoryTests
    {
        private const string DatabasePrefix = "ScrapeJob_";

        private static AppDbContext CreateContext(string dbName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(DatabasePrefix + dbName)
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
        public async Task CreateJobAsync_validJob_returnsPositiveId()
        {
            await using AppDbContext context = CreateContext(nameof(CreateJobAsync_validJob_returnsPositiveId));

            ScrapeJob scrapeJob = new ScrapeJob
            {
                SearchQuery = "action movies",
                MaxResults = 10,
                Status = "pending",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            int id = await repository.CreateJobAsync(scrapeJob);

            Assert.True(id > 0);
        }

        [Fact]
        public async Task CreateJobAsync_validJob_insertsOneRecord()
        {
            await using AppDbContext context = CreateContext(nameof(CreateJobAsync_validJob_insertsOneRecord));

            ScrapeJob scrapeJob = new ScrapeJob
            {
                SearchQuery = "action movies",
                MaxResults = 10,
                Status = "pending",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            await repository.CreateJobAsync(scrapeJob);

            int count = await context.ScrapeJobs.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task UpdateJobAsync_existingJob_updatesStatus()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateJobAsync_existingJob_updatesStatus));

            ScrapeJob scrapeJob = new ScrapeJob
            {
                SearchQuery = "action movies",
                MaxResults = 10,
                Status = "pending",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(scrapeJob);
            await context.SaveChangesAsync();

            scrapeJob.Status = "completed";
            scrapeJob.MoviesFound = 5;
            scrapeJob.CompletedAt = DateTime.UtcNow;

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            await repository.UpdateJobAsync(scrapeJob);

            ScrapeJob? updatedJob = await context.ScrapeJobs.FindAsync(scrapeJob.Id);

            Assert.Equal("completed", updatedJob!.Status);
        }

        [Fact]
        public async Task UpdateJobAsync_existingJob_updatesMoviesFound()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateJobAsync_existingJob_updatesMoviesFound));

            ScrapeJob scrapeJob = new ScrapeJob
            {
                SearchQuery = "action movies",
                MaxResults = 10,
                Status = "pending",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(scrapeJob);
            await context.SaveChangesAsync();

            scrapeJob.Status = "completed";
            scrapeJob.MoviesFound = 5;
            scrapeJob.CompletedAt = DateTime.UtcNow;

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            await repository.UpdateJobAsync(scrapeJob);

            ScrapeJob? updatedJob = await context.ScrapeJobs.FindAsync(scrapeJob.Id);

            Assert.Equal(5, updatedJob!.MoviesFound);
        }

        [Fact]
        public async Task UpdateJobAsync_nonExistentJob_doesNotInsertRecord()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateJobAsync_nonExistentJob_doesNotInsertRecord));

            ScrapeJob scrapeJob = new ScrapeJob
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

            await repository.UpdateJobAsync(scrapeJob);

            int count = await context.ScrapeJobs.CountAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task AddLogEntryAsync_validLog_insertsLog()
        {
            await using AppDbContext context = CreateContext(nameof(AddLogEntryAsync_validLog_insertsLog));

            ScrapeJob scrapeJob = new ScrapeJob
            {
                SearchQuery = "test",
                MaxResults = 5,
                Status = "running",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(scrapeJob);
            await context.SaveChangesAsync();

            ScrapeJobLog scrapeJobLog = new ScrapeJobLog
            {
                ScrapeJob = scrapeJob,
                Level = "Info",
                Message = "Starting scrape",
                Timestamp = DateTime.UtcNow,
            };

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            await repository.AddLogEntryAsync(scrapeJobLog);

            int count = await context.ScrapeJobLogs.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task AddLogEntryAsync_validLog_storesCorrectMessage()
        {
            await using AppDbContext context = CreateContext(nameof(AddLogEntryAsync_validLog_storesCorrectMessage));

            ScrapeJob scrapeJob = new ScrapeJob
            {
                SearchQuery = "test",
                MaxResults = 5,
                Status = "running",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(scrapeJob);
            await context.SaveChangesAsync();

            ScrapeJobLog scrapeJobLog = new ScrapeJobLog
            {
                ScrapeJob = scrapeJob,
                Level = "Info",
                Message = "Starting scrape",
                Timestamp = DateTime.UtcNow,
            };

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            await repository.AddLogEntryAsync(scrapeJobLog);

            ScrapeJobLog? insertedLog = await context.ScrapeJobLogs.FirstOrDefaultAsync();

            Assert.Equal("Starting scrape", insertedLog!.Message);
        }

        [Fact]
        public async Task GetAllJobsAsync_jobsExist_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllJobsAsync_jobsExist_returnsCorrectCount));

            context.ScrapeJobs.AddRange(
                new ScrapeJob
                {
                    SearchQuery = "old",
                    MaxResults = 5,
                    Status = "completed",
                    MoviesFound = 0,
                    ReelsCreated = 0,
                    StartedAt = DateTime.UtcNow.AddDays(-2),
                },
                new ScrapeJob
                {
                    SearchQuery = "new",
                    MaxResults = 5,
                    Status = "pending",
                    MoviesFound = 0,
                    ReelsCreated = 0,
                    StartedAt = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<ScrapeJob> result = await repository.GetAllJobsAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllJobsAsync_jobsExist_returnsNewestFirst()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllJobsAsync_jobsExist_returnsNewestFirst));

            context.ScrapeJobs.AddRange(
                new ScrapeJob
                {
                    SearchQuery = "old",
                    MaxResults = 5,
                    Status = "completed",
                    MoviesFound = 0,
                    ReelsCreated = 0,
                    StartedAt = DateTime.UtcNow.AddDays(-2),
                },
                new ScrapeJob
                {
                    SearchQuery = "new",
                    MaxResults = 5,
                    Status = "pending",
                    MoviesFound = 0,
                    ReelsCreated = 0,
                    StartedAt = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<ScrapeJob> result = await repository.GetAllJobsAsync();

            Assert.Equal("new", result[0].SearchQuery);
        }

        [Fact]
        public async Task GetLogsForJobAsync_logsExist_returnsSingleLog()
        {
            await using AppDbContext context = CreateContext(nameof(GetLogsForJobAsync_logsExist_returnsSingleLog));

            ScrapeJob scrapeJob = new ScrapeJob
            {
                SearchQuery = "test",
                MaxResults = 5,
                Status = "running",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(scrapeJob);
            await context.SaveChangesAsync();

            context.ScrapeJobLogs.Add(new ScrapeJobLog
            {
                ScrapeJob = scrapeJob,
                Level = "Info",
                Message = "Log message",
                Timestamp = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<ScrapeJobLog> result = await repository.GetLogsForJobAsync(scrapeJob.Id);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetLogsForJobAsync_noLogsForJob_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetLogsForJobAsync_noLogsForJob_returnsEmpty));

            ScrapeJob scrapeJob = new ScrapeJob
            {
                SearchQuery = "test",
                MaxResults = 5,
                Status = "running",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(scrapeJob);
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<ScrapeJobLog> result = await repository.GetLogsForJobAsync(scrapeJob.Id);

            Assert.Empty(result);
        }

        [Fact]
        public async Task ReelExistsByVideoUrlAsync_reelExists_returnsTrue()
        {
            await using AppDbContext context = CreateContext(nameof(ReelExistsByVideoUrlAsync_reelExists_returnsTrue));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.Reels.Add(new Reel
            {
                VideoUrl = "http://exists.url",
                ThumbnailUrl = "thumbnail",
                Title = "reel",
                Caption = "caption",
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
        public async Task FindMovieByTitleAsync_movieExists_returnsNotNull()
        {
            await using AppDbContext context = CreateContext(nameof(FindMovieByTitleAsync_movieExists_returnsNotNull));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            int? result = await repository.FindMovieByTitleAsync("Test Movie");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindMovieByTitleAsync_movieExists_returnsCorrectId()
        {
            await using AppDbContext context = CreateContext(nameof(FindMovieByTitleAsync_movieExists_returnsCorrectId));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            int? result = await repository.FindMovieByTitleAsync("Test Movie");

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
        public async Task GetDashboardStatsAsync_withData_returnsCorrectTotalMovies()
        {
            await using AppDbContext context = CreateContext(nameof(GetDashboardStatsAsync_withData_returnsCorrectTotalMovies));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.Reels.Add(new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "thumbnail",
                Title = "reel",
                Caption = "caption",
                FeatureDurationSeconds = 10m,
                Source = "scraped",
                CreatedAt = DateTime.UtcNow,
                Movie = movie,
                CreatorUser = user,
            });

            context.ScrapeJobs.Add(new ScrapeJob
            {
                SearchQuery = "query",
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
        }

        [Fact]
        public async Task GetDashboardStatsAsync_withData_returnsCorrectTotalReels()
        {
            await using AppDbContext context = CreateContext(nameof(GetDashboardStatsAsync_withData_returnsCorrectTotalReels));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.Reels.Add(new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "thumbnail",
                Title = "reel",
                Caption = "caption",
                FeatureDurationSeconds = 10m,
                Source = "scraped",
                CreatedAt = DateTime.UtcNow,
                Movie = movie,
                CreatorUser = user,
            });

            context.ScrapeJobs.Add(new ScrapeJob
            {
                SearchQuery = "query",
                MaxResults = 5,
                Status = "completed",
                MoviesFound = 1,
                ReelsCreated = 1,
                StartedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            DashboardStatsModel result = await repository.GetDashboardStatsAsync();

            Assert.Equal(1, result.TotalReels);
        }

        [Fact]
        public async Task GetDashboardStatsAsync_withData_returnsCorrectCompletedJobs()
        {
            await using AppDbContext context = CreateContext(nameof(GetDashboardStatsAsync_withData_returnsCorrectCompletedJobs));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.ScrapeJobs.Add(new ScrapeJob
            {
                SearchQuery = "query",
                MaxResults = 5,
                Status = "completed",
                MoviesFound = 1,
                ReelsCreated = 1,
                StartedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            DashboardStatsModel result = await repository.GetDashboardStatsAsync();

            Assert.Equal(1, result.CompletedJobs);
        }

        [Fact]
        public async Task GetDashboardStatsAsync_withData_returnsZeroRunningJobs()
        {
            await using AppDbContext context = CreateContext(nameof(GetDashboardStatsAsync_withData_returnsZeroRunningJobs));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.ScrapeJobs.Add(new ScrapeJob
            {
                SearchQuery = "query",
                MaxResults = 5,
                Status = "completed",
                MoviesFound = 1,
                ReelsCreated = 1,
                StartedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            DashboardStatsModel result = await repository.GetDashboardStatsAsync();

            Assert.Equal(0, result.RunningJobs);
        }

        [Fact]
        public async Task GetDashboardStatsAsync_withData_returnsZeroFailedJobs()
        {
            await using AppDbContext context = CreateContext(nameof(GetDashboardStatsAsync_withData_returnsZeroFailedJobs));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.ScrapeJobs.Add(new ScrapeJob
            {
                SearchQuery = "query",
                MaxResults = 5,
                Status = "completed",
                MoviesFound = 1,
                ReelsCreated = 1,
                StartedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            DashboardStatsModel result = await repository.GetDashboardStatsAsync();

            Assert.Equal(0, result.FailedJobs);
        }

        [Fact]
        public async Task SearchMoviesByNameAsync_matchingMovies_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext(nameof(SearchMoviesByNameAsync_matchingMovies_returnsCorrectCount));

            context.Movies.AddRange(
                new Movie
                {
                    Title = "Action Hero",
                    Description = "desc",
                    Rating = 7m,
                    Price = 9.99m,
                    PrimaryGenre = "Action",
                    ReleaseYear = 2020,
                    Synopsis = "synopsis",
                },
                new Movie
                {
                    Title = "Action Star",
                    Description = "desc",
                    Rating = 8m,
                    Price = 9.99m,
                    PrimaryGenre = "Action",
                    ReleaseYear = 2021,
                    Synopsis = "synopsis",
                },
                new Movie
                {
                    Title = "Drama Queen",
                    Description = "desc",
                    Rating = 6m,
                    Price = 9.99m,
                    PrimaryGenre = "Drama",
                    ReleaseYear = 2019,
                    Synopsis = "synopsis",
                });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Movie> result = await repository.SearchMoviesByNameAsync("Action");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SearchMoviesByNameAsync_matchingMovies_returnsOrderedByTitle()
        {
            await using AppDbContext context = CreateContext(nameof(SearchMoviesByNameAsync_matchingMovies_returnsOrderedByTitle));

            context.Movies.AddRange(
                new Movie
                {
                    Title = "Zebra Action",
                    Description = "desc",
                    Rating = 7m,
                    Price = 9.99m,
                    PrimaryGenre = "Action",
                    ReleaseYear = 2020,
                    Synopsis = "synopsis",
                },
                new Movie
                {
                    Title = "Alpha Action",
                    Description = "desc",
                    Rating = 8m,
                    Price = 9.99m,
                    PrimaryGenre = "Action",
                    ReleaseYear = 2021,
                    Synopsis = "synopsis",
                });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Movie> result = await repository.SearchMoviesByNameAsync("Action");

            Assert.Equal("Alpha Action", result[0].Title);
        }

        [Fact]
        public async Task SearchMoviesByNameAsync_noMatchingMovies_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(SearchMoviesByNameAsync_noMatchingMovies_returnsEmpty));

            context.Movies.Add(new Movie
            {
                Title = "Drama Queen",
                Description = "desc",
                Rating = 6m,
                Price = 9.99m,
                PrimaryGenre = "Drama",
                ReleaseYear = 2019,
                Synopsis = "synopsis",
            });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Movie> result = await repository.SearchMoviesByNameAsync("Action");

            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchMoviesByNameAsync_partialMatch_returnsMatchingMovies()
        {
            await using AppDbContext context = CreateContext(nameof(SearchMoviesByNameAsync_partialMatch_returnsMatchingMovies));

            context.Movies.AddRange(
                new Movie
                {
                    Title = "The Dark Knight",
                    Description = "desc",
                    Rating = 9m,
                    Price = 9.99m,
                    PrimaryGenre = "Action",
                    ReleaseYear = 2008,
                    Synopsis = "synopsis",
                },
                new Movie
                {
                    Title = "Darkest Hour",
                    Description = "desc",
                    Rating = 7m,
                    Price = 9.99m,
                    PrimaryGenre = "Drama",
                    ReleaseYear = 2017,
                    Synopsis = "synopsis",
                },
                new Movie
                {
                    Title = "Inception",
                    Description = "desc",
                    Rating = 9m,
                    Price = 9.99m,
                    PrimaryGenre = "Sci-Fi",
                    ReleaseYear = 2010,
                    Synopsis = "synopsis",
                });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Movie> result = await repository.SearchMoviesByNameAsync("Dark");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SearchMoviesByNameAsync_emptyDatabase_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(SearchMoviesByNameAsync_emptyDatabase_returnsEmpty));

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Movie> result = await repository.SearchMoviesByNameAsync("Action");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllLogsAsync_noLogs_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllLogsAsync_noLogs_returnsEmpty));

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<ScrapeJobLog> result = await repository.GetAllLogsAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllLogsAsync_logsExist_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllLogsAsync_logsExist_returnsCorrectCount));

            ScrapeJob scrapeJob = new ScrapeJob
            {
                SearchQuery = "test",
                MaxResults = 5,
                Status = "running",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(scrapeJob);
            await context.SaveChangesAsync();

            context.ScrapeJobLogs.AddRange(
                new ScrapeJobLog
                {
                    ScrapeJob = scrapeJob,
                    Level = "Info",
                    Message = "First",
                    Timestamp = DateTime.UtcNow.AddSeconds(-2),
                },
                new ScrapeJobLog
                {
                    ScrapeJob = scrapeJob,
                    Level = "Info",
                    Message = "Second",
                    Timestamp = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<ScrapeJobLog> result = await repository.GetAllLogsAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllLogsAsync_logsExist_returnsNewestFirst()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllLogsAsync_logsExist_returnsNewestFirst));

            ScrapeJob scrapeJob = new ScrapeJob
            {
                SearchQuery = "test",
                MaxResults = 5,
                Status = "running",
                MoviesFound = 0,
                ReelsCreated = 0,
                StartedAt = DateTime.UtcNow,
            };

            context.ScrapeJobs.Add(scrapeJob);
            await context.SaveChangesAsync();

            context.ScrapeJobLogs.AddRange(
                new ScrapeJobLog
                {
                    ScrapeJob = scrapeJob,
                    Level = "Info",
                    Message = "Older",
                    Timestamp = DateTime.UtcNow.AddSeconds(-10),
                },
                new ScrapeJobLog
                {
                    ScrapeJob = scrapeJob,
                    Level = "Info",
                    Message = "Newer",
                    Timestamp = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<ScrapeJobLog> result = await repository.GetAllLogsAsync();

            Assert.Equal("Newer", result[0].Message);
        }

        [Fact]
        public async Task InsertScrapedReelAsync_validReel_returnsPositiveId()
        {
            await using AppDbContext context = CreateContext(nameof(InsertScrapedReelAsync_validReel_returnsPositiveId));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            Reel reel = new Reel
            {
                VideoUrl = "http://scraped.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "Scraped Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "scraped",
                Movie = movie,
                CreatorUser = user,
            };

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            int id = await repository.InsertScrapedReelAsync(reel);

            Assert.True(id > 0);
        }

        [Fact]
        public async Task InsertScrapedReelAsync_validReel_insertsOneRecord()
        {
            await using AppDbContext context = CreateContext(nameof(InsertScrapedReelAsync_validReel_insertsOneRecord));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            Reel reel = new Reel
            {
                VideoUrl = "http://scraped.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "Scraped Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "scraped",
                Movie = movie,
                CreatorUser = user,
            };

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            await repository.InsertScrapedReelAsync(reel);

            int count = await context.Reels.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task InsertScrapedReelAsync_validReel_setsCreatedAt()
        {
            await using AppDbContext context = CreateContext(nameof(InsertScrapedReelAsync_validReel_setsCreatedAt));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            DateTime beforeInsert = DateTime.UtcNow.AddSeconds(-1);

            Reel reel = new Reel
            {
                VideoUrl = "http://scraped.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "Scraped Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "scraped",
                Movie = movie,
                CreatorUser = user,
            };

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            await repository.InsertScrapedReelAsync(reel);

            Reel? inserted = await context.Reels.FirstOrDefaultAsync();

            Assert.True(inserted!.CreatedAt >= beforeInsert);
        }

        [Fact]
        public async Task GetAllMoviesAsync_noMovies_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllMoviesAsync_noMovies_returnsEmpty));

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Movie> result = await repository.GetAllMoviesAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllMoviesAsync_moviesExist_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllMoviesAsync_moviesExist_returnsCorrectCount));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Movie> result = await repository.GetAllMoviesAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllMoviesAsync_moviesExist_returnsOrderedByTitle()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllMoviesAsync_moviesExist_returnsOrderedByTitle));

            context.Movies.AddRange(
                new Movie
                {
                    Title = "Zebra Movie",
                    Description = "desc",
                    Rating = 5m,
                    Price = 9.99m,
                    PrimaryGenre = "Action",
                    ReleaseYear = 2020,
                    Synopsis = "synopsis",
                },
                new Movie
                {
                    Title = "Alpha Movie",
                    Description = "desc",
                    Rating = 5m,
                    Price = 9.99m,
                    PrimaryGenre = "Drama",
                    ReleaseYear = 2021,
                    Synopsis = "synopsis",
                });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Movie> result = await repository.GetAllMoviesAsync();

            Assert.Equal("Alpha Movie", result[0].Title);
        }

        [Fact]
        public async Task GetAllReelsAsync_noReels_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllReelsAsync_noReels_returnsEmpty));

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Reel> result = await repository.GetAllReelsAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllReelsAsync_reelsExist_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllReelsAsync_reelsExist_returnsCorrectCount));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.Reels.Add(new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "Test Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "scraped",
                CreatedAt = DateTime.UtcNow,
                Movie = movie,
                CreatorUser = user,
            });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Reel> result = await repository.GetAllReelsAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllReelsAsync_reelsExist_returnsNewestFirst()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllReelsAsync_reelsExist_returnsNewestFirst));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.Reels.AddRange(
                new Reel
                {
                    VideoUrl = "http://older.url",
                    ThumbnailUrl = "http://thumb.url",
                    Title = "Older Reel",
                    Caption = "caption",
                    FeatureDurationSeconds = 30m,
                    Source = "scraped",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    Movie = movie,
                    CreatorUser = user,
                },
                new Reel
                {
                    VideoUrl = "http://newer.url",
                    ThumbnailUrl = "http://thumb.url",
                    Title = "Newer Reel",
                    Caption = "caption",
                    FeatureDurationSeconds = 30m,
                    Source = "scraped",
                    CreatedAt = DateTime.UtcNow,
                    Movie = movie,
                    CreatorUser = user,
                });
            await context.SaveChangesAsync();

            ScrapeJobRepository repository = new ScrapeJobRepository(context);

            IList<Reel> result = await repository.GetAllReelsAsync();

            Assert.Equal("Newer Reel", result[0].Title);
        }
    }
}
