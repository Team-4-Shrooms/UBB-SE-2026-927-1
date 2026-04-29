using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MovieApp.Tests.Repositories
{
    public class MovieTournamentRepositoryTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        private static async Task<(User user, Movie movie)> SeedUserAndMovieWithPreference(AppDbContext context, int changeFromPrevious = 1)
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

            context.UserMoviePreferences.Add(new UserMoviePreference
            {
                User = user,
                Movie = movie,
                Score = 5m,
                LastModified = DateTime.UtcNow,
                ChangeFromPreviousValue = changeFromPrevious,
            });
            await context.SaveChangesAsync();

            return (user, movie);
        }

        [Fact]
        public async Task GetTournamentPoolSizeAsync_userHasPositivePreferences_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext(nameof(GetTournamentPoolSizeAsync_userHasPositivePreferences_returnsCorrectCount));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 1);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            int result = await repository.GetTournamentPoolSizeAsync(user.Id);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetTournamentPoolSizeAsync_userHasNoPositivePreferences_returnsZero()
        {
            await using AppDbContext context = CreateContext(nameof(GetTournamentPoolSizeAsync_userHasNoPositivePreferences_returnsZero));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 0);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            int result = await repository.GetTournamentPoolSizeAsync(user.Id);

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetTournamentPoolSizeAsync_noPreferences_returnsZero()
        {
            await using AppDbContext context = CreateContext(nameof(GetTournamentPoolSizeAsync_noPreferences_returnsZero));

            User user = new User
            {
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = "hash",
                Balance = 0m,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            int result = await repository.GetTournamentPoolSizeAsync(user.Id);

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetTournamentPoolSizeAsync_negativeChangeFromPrevious_isExcludedFromCount()
        {
            await using AppDbContext context = CreateContext(nameof(GetTournamentPoolSizeAsync_negativeChangeFromPrevious_isExcludedFromCount));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: -1);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            int result = await repository.GetTournamentPoolSizeAsync(user.Id);

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetTournamentPoolAsync_validPoolSize_returnsSingleResult()
        {
            await using AppDbContext context = CreateContext(nameof(GetTournamentPoolAsync_validPoolSize_returnsSingleResult));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 1);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            List<Movie> result = await repository.GetTournamentPoolAsync(user.Id, 4);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetTournamentPoolAsync_validPoolSize_returnsCorrectMovieId()
        {
            await using AppDbContext context = CreateContext(nameof(GetTournamentPoolAsync_validPoolSize_returnsCorrectMovieId));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 1);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            List<Movie> result = await repository.GetTournamentPoolAsync(user.Id, 4);

            Assert.Equal(movie.Id, result[0].Id);
        }

        [Fact]
        public async Task GetTournamentPoolAsync_poolSizeLargerThanAvailable_returnsAllAvailable()
        {
            await using AppDbContext context = CreateContext(nameof(GetTournamentPoolAsync_poolSizeLargerThanAvailable_returnsAllAvailable));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 1);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            List<Movie> result = await repository.GetTournamentPoolAsync(user.Id, 100);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetTournamentPoolAsync_zeroChangeFromPrevious_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetTournamentPoolAsync_zeroChangeFromPrevious_returnsEmpty));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 0);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            List<Movie> result = await repository.GetTournamentPoolAsync(user.Id, 10);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTournamentPoolAsync_noPreferences_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetTournamentPoolAsync_noPreferences_returnsEmpty));

            User user = new User
            {
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = "hash",
                Balance = 0m,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            List<Movie> result = await repository.GetTournamentPoolAsync(user.Id, 4);

            Assert.Empty(result);
        }

        [Fact]
        public async Task BoostMovieScoreAsync_existingPreference_increasesScore()
        {
            await using AppDbContext context = CreateContext(nameof(BoostMovieScoreAsync_existingPreference_increasesScore));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 1);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            await repository.BoostMovieScoreAsync(user.Id, movie.Id, 2m);

            UserMoviePreference? userPreference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(preference => preference.User.Id == user.Id && preference.Movie.Id == movie.Id);

            Assert.Equal(7m, userPreference!.Score);
        }

        [Fact]
        public async Task BoostMovieScoreAsync_existingPreference_updatesChangeFromPreviousValue()
        {
            await using AppDbContext context = CreateContext(nameof(BoostMovieScoreAsync_existingPreference_updatesChangeFromPreviousValue));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 1);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            await repository.BoostMovieScoreAsync(user.Id, movie.Id, 3m);

            UserMoviePreference? userPreference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(preference => preference.User.Id == user.Id && preference.Movie.Id == movie.Id);

            Assert.Equal(3, userPreference!.ChangeFromPreviousValue);
        }

        [Fact]
        public async Task BoostMovieScoreAsync_existingPreference_updatesLastModified()
        {
            await using AppDbContext context = CreateContext(nameof(BoostMovieScoreAsync_existingPreference_updatesLastModified));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 1);

            DateTime beforeBoost = DateTime.UtcNow.AddSeconds(-1);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            await repository.BoostMovieScoreAsync(user.Id, movie.Id, 2m);

            UserMoviePreference? userPreference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(preference => preference.User.Id == user.Id && preference.Movie.Id == movie.Id);

            Assert.True(userPreference!.LastModified >= beforeBoost);
        }

        [Fact]
        public async Task BoostMovieScoreAsync_nonExistentPreference_doesNotInsertPreference()
        {
            await using AppDbContext context = CreateContext(nameof(BoostMovieScoreAsync_nonExistentPreference_doesNotInsertPreference));

            User user = new User
            {
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = "hash",
                Balance = 0m,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            await repository.BoostMovieScoreAsync(user.Id, 999, 2m);

            int count = await context.UserMoviePreferences.CountAsync();

            Assert.Equal(0, count);
        }
    }
}
