using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MovieApp.Tests.Repositories
{
    public class RecommendationRepositoryTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        private static async Task<(User user, Movie movie, Reel reel)> SeedUserMovieAndReel(AppDbContext context)
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

            Reel reel = new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "Test Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "upload",
                CreatedAt = DateTime.UtcNow,
                Movie = movie,
                CreatorUser = user,
            };

            context.Reels.Add(reel);
            await context.SaveChangesAsync();

            return (user, movie, reel);
        }

        [Fact]
        public async Task UserHasPreferencesAsync_noPreferences_returnsFalse()
        {
            await using AppDbContext context = CreateContext(nameof(UserHasPreferencesAsync_noPreferences_returnsFalse));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            RecommendationRepository repository = new RecommendationRepository(context);

            bool result = await repository.UserHasPreferencesAsync(user.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task UserHasPreferencesAsync_preferencesExist_returnsTrue()
        {
            await using AppDbContext context = CreateContext(nameof(UserHasPreferencesAsync_preferencesExist_returnsTrue));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserMoviePreferences.Add(new UserMoviePreference
            {
                User = user,
                Movie = movie,
                Score = 7m,
                LastModified = DateTime.UtcNow,
                ChangeFromPreviousValue = 1,
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            bool result = await repository.UserHasPreferencesAsync(user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task UserHasPreferencesAsync_wrongUserId_returnsFalse()
        {
            await using AppDbContext context = CreateContext(nameof(UserHasPreferencesAsync_wrongUserId_returnsFalse));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserMoviePreferences.Add(new UserMoviePreference
            {
                User = user,
                Movie = movie,
                Score = 7m,
                LastModified = DateTime.UtcNow,
                ChangeFromPreviousValue = 1,
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            bool result = await repository.UserHasPreferencesAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task GetAllReelsAsync_noReels_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllReelsAsync_noReels_returnsEmpty));

            RecommendationRepository repository = new RecommendationRepository(context);

            IList<Reel> result = await repository.GetAllReelsAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllReelsAsync_reelsExist_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllReelsAsync_reelsExist_returnsCorrectCount));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            RecommendationRepository repository = new RecommendationRepository(context);

            IList<Reel> result = await repository.GetAllReelsAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllReelsAsync_reelsExist_returnsCorrectReelId()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllReelsAsync_reelsExist_returnsCorrectReelId));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            RecommendationRepository repository = new RecommendationRepository(context);

            IList<Reel> result = await repository.GetAllReelsAsync();

            Assert.Equal(reel.Id, result[0].Id);
        }

        [Fact]
        public async Task GetAllReelsAsync_reelsExist_includesMovie()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllReelsAsync_reelsExist_includesMovie));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            RecommendationRepository repository = new RecommendationRepository(context);

            IList<Reel> result = await repository.GetAllReelsAsync();

            Assert.NotNull(result[0].Movie);
        }

        [Fact]
        public async Task GetUserPreferenceScoresAsync_noPreferences_returnsEmptyDictionary()
        {
            await using AppDbContext context = CreateContext(nameof(GetUserPreferenceScoresAsync_noPreferences_returnsEmptyDictionary));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            RecommendationRepository repository = new RecommendationRepository(context);

            Dictionary<int, decimal> result = await repository.GetUserPreferenceScoresAsync(user.Id);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserPreferenceScoresAsync_preferencesExist_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext(nameof(GetUserPreferenceScoresAsync_preferencesExist_returnsCorrectCount));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserMoviePreferences.Add(new UserMoviePreference
            {
                User = user,
                Movie = movie,
                Score = 7m,
                LastModified = DateTime.UtcNow,
                ChangeFromPreviousValue = 1,
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            Dictionary<int, decimal> result = await repository.GetUserPreferenceScoresAsync(user.Id);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetUserPreferenceScoresAsync_preferencesExist_returnsCorrectScore()
        {
            await using AppDbContext context = CreateContext(nameof(GetUserPreferenceScoresAsync_preferencesExist_returnsCorrectScore));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserMoviePreferences.Add(new UserMoviePreference
            {
                User = user,
                Movie = movie,
                Score = 7m,
                LastModified = DateTime.UtcNow,
                ChangeFromPreviousValue = 1,
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            Dictionary<int, decimal> result = await repository.GetUserPreferenceScoresAsync(user.Id);

            Assert.Equal(7m, result[movie.Id]);
        }

        [Fact]
        public async Task GetUserPreferenceScoresAsync_wrongUserId_returnsEmptyDictionary()
        {
            await using AppDbContext context = CreateContext(nameof(GetUserPreferenceScoresAsync_wrongUserId_returnsEmptyDictionary));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserMoviePreferences.Add(new UserMoviePreference
            {
                User = user,
                Movie = movie,
                Score = 7m,
                LastModified = DateTime.UtcNow,
                ChangeFromPreviousValue = 1,
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            Dictionary<int, decimal> result = await repository.GetUserPreferenceScoresAsync(999);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllLikeCountsAsync_noLikes_returnsEmptyDictionary()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllLikeCountsAsync_noLikes_returnsEmptyDictionary));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = false,
                WatchDurationSeconds = 10m,
                WatchPercentage = 33m,
                ViewedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            Dictionary<int, int> result = await repository.GetAllLikeCountsAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllLikeCountsAsync_oneLike_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllLikeCountsAsync_oneLike_returnsCorrectCount));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = true,
                WatchDurationSeconds = 10m,
                WatchPercentage = 33m,
                ViewedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            Dictionary<int, int> result = await repository.GetAllLikeCountsAsync();

            Assert.Equal(1, result[reel.Id]);
        }

        [Fact]
        public async Task GetAllLikeCountsAsync_oneLike_returnsSingleEntry()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllLikeCountsAsync_oneLike_returnsSingleEntry));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = true,
                WatchDurationSeconds = 10m,
                WatchPercentage = 33m,
                ViewedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            Dictionary<int, int> result = await repository.GetAllLikeCountsAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetLikesWithinDaysAsync_likeWithinRange_returnsInteraction()
        {
            await using AppDbContext context = CreateContext(nameof(GetLikesWithinDaysAsync_likeWithinRange_returnsInteraction));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = true,
                WatchDurationSeconds = 10m,
                WatchPercentage = 33m,
                ViewedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            List<UserReelInteraction> result = await repository.GetLikesWithinDaysAsync(7);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetLikesWithinDaysAsync_likeOutsideRange_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetLikesWithinDaysAsync_likeOutsideRange_returnsEmpty));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = true,
                WatchDurationSeconds = 10m,
                WatchPercentage = 33m,
                ViewedAt = DateTime.UtcNow.AddDays(-10),
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            List<UserReelInteraction> result = await repository.GetLikesWithinDaysAsync(7);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetLikesWithinDaysAsync_unlikedInteractionWithinRange_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetLikesWithinDaysAsync_unlikedInteractionWithinRange_returnsEmpty));
            (User user, Movie movie, Reel reel) = await SeedUserMovieAndReel(context);

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = false,
                WatchDurationSeconds = 10m,
                WatchPercentage = 33m,
                ViewedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            RecommendationRepository repository = new RecommendationRepository(context);

            List<UserReelInteraction> result = await repository.GetLikesWithinDaysAsync(7);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetLikesWithinDaysAsync_noInteractions_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetLikesWithinDaysAsync_noInteractions_returnsEmpty));

            RecommendationRepository repository = new RecommendationRepository(context);

            List<UserReelInteraction> result = await repository.GetLikesWithinDaysAsync(7);

            Assert.Empty(result);
        }
    }
}
