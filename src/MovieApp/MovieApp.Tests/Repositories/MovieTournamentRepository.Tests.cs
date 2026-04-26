using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;
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
        public async Task GetTournamentPoolAsync_validPoolSize_returnsMovies()
        {
            await using AppDbContext context = CreateContext(nameof(GetTournamentPoolAsync_validPoolSize_returnsMovies));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 1);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            List<Movie> result = await repository.GetTournamentPoolAsync(user.Id, 4);

            Assert.Single(result);
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
        public async Task BoostMovieScoreAsync_existingPreference_increasesScore()
        {
            await using AppDbContext context = CreateContext(nameof(BoostMovieScoreAsync_existingPreference_increasesScore));
            (User user, Movie movie) = await SeedUserAndMovieWithPreference(context, changeFromPrevious: 1);

            MovieTournamentRepository repository = new MovieTournamentRepository(context);

            await repository.BoostMovieScoreAsync(user.Id, movie.Id, 2m);

            UserMoviePreference? preference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(p => p.User.Id == user.Id && p.Movie.Id == movie.Id);

            Assert.NotNull(preference);
            Assert.Equal(7m, preference.Score);
        }

        [Fact]
        public async Task BoostMovieScoreAsync_nonExistentPreference_doesNotThrow()
        {
            await using AppDbContext context = CreateContext(nameof(BoostMovieScoreAsync_nonExistentPreference_doesNotThrow));

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

            Assert.Equal(0, await context.UserMoviePreferences.CountAsync());
        }
    }
}
