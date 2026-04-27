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
        public async Task PreferenceExistsAsync_wrongUserId_returnsFalse()
        {
            await using AppDbContext context = CreateContext(nameof(PreferenceExistsAsync_wrongUserId_returnsFalse));
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

            bool result = await repository.PreferenceExistsAsync(999, movie.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task PreferenceExistsAsync_wrongMovieId_returnsFalse()
        {
            await using AppDbContext context = CreateContext(nameof(PreferenceExistsAsync_wrongMovieId_returnsFalse));
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

            bool result = await repository.PreferenceExistsAsync(user.Id, 999);

            Assert.False(result);
        }

        [Fact]
        public async Task InsertPreferenceAsync_validInput_insertsPreference()
        {
            await using AppDbContext context = CreateContext(nameof(InsertPreferenceAsync_validInput_insertsPreference));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            PreferenceRepository repository = new PreferenceRepository(context);

            await repository.InsertPreferenceAsync(user.Id, movie.Id, 3m);

            int count = await context.UserMoviePreferences.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task InsertPreferenceAsync_validInput_storesCorrectScore()
        {
            await using AppDbContext context = CreateContext(nameof(InsertPreferenceAsync_validInput_storesCorrectScore));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            PreferenceRepository repository = new PreferenceRepository(context);

            await repository.InsertPreferenceAsync(user.Id, movie.Id, 3m);

            UserMoviePreference? userPreference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(preference => preference.User.Id == user.Id && preference.Movie.Id == movie.Id);

            Assert.Equal(3m, userPreference!.Score);
        }

        [Fact]
        public async Task InsertPreferenceAsync_validInput_setsChangeFromPreviousValueToZero()
        {
            await using AppDbContext context = CreateContext(nameof(InsertPreferenceAsync_validInput_setsChangeFromPreviousValueToZero));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            PreferenceRepository repository = new PreferenceRepository(context);

            await repository.InsertPreferenceAsync(user.Id, movie.Id, 3m);

            UserMoviePreference? userPreference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(preference => preference.User.Id == user.Id && preference.Movie.Id == movie.Id);

            Assert.Equal(0, userPreference!.ChangeFromPreviousValue);
        }

        [Fact]
        public async Task InsertPreferenceAsync_validInput_linksCorrectUser()
        {
            await using AppDbContext context = CreateContext(nameof(InsertPreferenceAsync_validInput_linksCorrectUser));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            PreferenceRepository repository = new PreferenceRepository(context);

            await repository.InsertPreferenceAsync(user.Id, movie.Id, 3m);

            UserMoviePreference? userPreference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(preference => preference.User.Id == user.Id && preference.Movie.Id == movie.Id);

            Assert.Equal(user.Id, userPreference!.User.Id);
        }

        [Fact]
        public async Task InsertPreferenceAsync_validInput_linksCorrectMovie()
        {
            await using AppDbContext context = CreateContext(nameof(InsertPreferenceAsync_validInput_linksCorrectMovie));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            PreferenceRepository repository = new PreferenceRepository(context);

            await repository.InsertPreferenceAsync(user.Id, movie.Id, 3m);

            UserMoviePreference? userPreference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(preference => preference.User.Id == user.Id && preference.Movie.Id == movie.Id);

            Assert.Equal(movie.Id, userPreference!.Movie.Id);
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

            UserMoviePreference? userPreference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(preference => preference.User.Id == user.Id && preference.Movie.Id == movie.Id);

            Assert.Equal(8m, userPreference!.Score);
        }

        [Fact]
        public async Task UpdatePreferenceAsync_existingPreference_updatesLastModified()
        {
            await using AppDbContext context = CreateContext(nameof(UpdatePreferenceAsync_existingPreference_updatesLastModified));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            context.UserMoviePreferences.Add(new UserMoviePreference
            {
                User = user,
                Movie = movie,
                Score = 5m,
                LastModified = DateTime.UtcNow.AddDays(-1),
            });
            await context.SaveChangesAsync();

            DateTime beforeUpdate = DateTime.UtcNow.AddSeconds(-1);

            PreferenceRepository repository = new PreferenceRepository(context);

            await repository.UpdatePreferenceAsync(user.Id, movie.Id, 3m);

            UserMoviePreference? userPreference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(preference => preference.User.Id == user.Id && preference.Movie.Id == movie.Id);

            Assert.True(userPreference!.LastModified >= beforeUpdate);
        }

        [Fact]
        public async Task UpdatePreferenceAsync_existingPreference_updatesChangeFromPreviousValue()
        {
            await using AppDbContext context = CreateContext(nameof(UpdatePreferenceAsync_existingPreference_updatesChangeFromPreviousValue));
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

            UserMoviePreference? userPreference = await context.UserMoviePreferences
                .FirstOrDefaultAsync(preference => preference.User.Id == user.Id && preference.Movie.Id == movie.Id);

            Assert.Equal(3, userPreference!.ChangeFromPreviousValue);
        }

        [Fact]
        public async Task UpdatePreferenceAsync_nonExistentPreference_doesNotInsertPreference()
        {
            await using AppDbContext context = CreateContext(nameof(UpdatePreferenceAsync_nonExistentPreference_doesNotInsertPreference));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            PreferenceRepository repository = new PreferenceRepository(context);

            await repository.UpdatePreferenceAsync(user.Id, movie.Id, 3m);

            int count = await context.UserMoviePreferences.CountAsync();

            Assert.Equal(0, count);
        }
    }
}
