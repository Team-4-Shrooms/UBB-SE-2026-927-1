using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;
using System.Threading.Tasks;
using System;

namespace MovieApp.Tests.Repositories
{
    public class PreferenceRepositoryTests
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
        public async Task PreferenceExistsAsync_noPreference_returnsFalse()
        {
            await using AppDbContext context = CreateContext(nameof(PreferenceExistsAsync_noPreference_returnsFalse));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            PreferenceRepository repository = new PreferenceRepository(context);

            bool result = await repository.PreferenceExistsAsync(user.Id, movie.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task PreferenceExistsAsync_preferenceExists_returnsTrue()
        {
            await using AppDbContext context = CreateContext(nameof(PreferenceExistsAsync_preferenceExists_returnsTrue));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.UserMoviePreferences.Add(new UserMoviePreference
            {
                User = user,
                Movie = movie,
                Score = 5m,
                LastModified = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            PreferenceRepository repository = new PreferenceRepository(context);

            bool result = await repository.PreferenceExistsAsync(user.Id, movie.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task InsertPreferenceAsync_validInput_insertsPreference()
        {
            await using AppDbContext context = CreateContext(nameof(InsertPreferenceAsync_validInput_insertsPreference));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            PreferenceRepository repository = new PreferenceRepository(context);

            await repository.InsertPreferenceAsync(user.Id, movie.Id, 3m);

            UserMoviePreference? preference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(p => p.User.Id == user.Id && p.Movie.Id == movie.Id);

            Assert.NotNull(preference);
            Assert.Equal(3m, preference.Score);
        }

        [Fact]
        public async Task UpdatePreferenceAsync_existingPreference_updatesScore()
        {
            await using AppDbContext context = CreateContext(nameof(UpdatePreferenceAsync_existingPreference_updatesScore));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.UserMoviePreferences.Add(new UserMoviePreference
            {
                User = user,
                Movie = movie,
                Score = 5m,
                LastModified = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            PreferenceRepository repository = new PreferenceRepository(context);

            await repository.UpdatePreferenceAsync(user.Id, movie.Id, 3m);

            UserMoviePreference? preference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(p => p.User.Id == user.Id && p.Movie.Id == movie.Id);

            Assert.NotNull(preference);
            Assert.Equal(8m, preference.Score);
        }

        [Fact]
        public async Task UpdatePreferenceAsync_nonExistentPreference_doesNotThrow()
        {
            await using AppDbContext context = CreateContext(nameof(UpdatePreferenceAsync_nonExistentPreference_doesNotThrow));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            PreferenceRepository repository = new PreferenceRepository(context);

            await repository.UpdatePreferenceAsync(user.Id, movie.Id, 3m);

            int count = await context.UserMoviePreferences.CountAsync();
            Assert.Equal(0, count);
        }
    }
}
