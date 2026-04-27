using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;
using System.Threading.Tasks;
using System;

namespace MovieApp.Tests.Repositories
{
    public class ProfileRepositoryTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        private static async Task<User> SeedUser(AppDbContext context)
        {
            User user = new User
            {
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = "hash",
                Balance = 0m,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        [Fact]
        public async Task GetProfileAsync_noProfile_returnsNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_noProfile_returnsNull));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile? result = await repository.GetProfileAsync(user.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetProfileAsync_profileExists_returnsNotNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_profileExists_returnsNotNull));
            User user = await SeedUser(context);

            context.UserProfiles.Add(new UserProfile
            {
                User = user,
                TotalLikes = 10,
                TotalWatchTimeSeconds = 3600,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 10,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile? result = await repository.GetProfileAsync(user.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetProfileAsync_profileExists_returnsCorrectTotalLikes()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_profileExists_returnsCorrectTotalLikes));
            User user = await SeedUser(context);

            context.UserProfiles.Add(new UserProfile
            {
                User = user,
                TotalLikes = 10,
                TotalWatchTimeSeconds = 3600,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 10,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile? result = await repository.GetProfileAsync(user.Id);

            Assert.Equal(10, result!.TotalLikes);
        }

        [Fact]
        public async Task GetProfileAsync_profileExists_returnsCorrectTotalWatchTime()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_profileExists_returnsCorrectTotalWatchTime));
            User user = await SeedUser(context);

            context.UserProfiles.Add(new UserProfile
            {
                User = user,
                TotalLikes = 10,
                TotalWatchTimeSeconds = 3600,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 10,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile? result = await repository.GetProfileAsync(user.Id);

            Assert.Equal(3600, result!.TotalWatchTimeSeconds);
        }

        [Fact]
        public async Task GetProfileAsync_wrongUserId_returnsNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_wrongUserId_returnsNull));
            User user = await SeedUser(context);

            context.UserProfiles.Add(new UserProfile
            {
                User = user,
                TotalLikes = 10,
                TotalWatchTimeSeconds = 3600,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 10,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile? result = await repository.GetProfileAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpsertProfileAsync_noExistingProfile_insertsProfile()
        {
            await using AppDbContext context = CreateContext(nameof(UpsertProfileAsync_noExistingProfile_insertsProfile));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile userProfile = new UserProfile
            {
                User = user,
                TotalLikes = 5,
                TotalWatchTimeSeconds = 1800,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 5,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            };

            await repository.UpsertProfileAsync(userProfile);

            int count = await context.UserProfiles.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task UpsertProfileAsync_noExistingProfile_storesCorrectTotalLikes()
        {
            await using AppDbContext context = CreateContext(nameof(UpsertProfileAsync_noExistingProfile_storesCorrectTotalLikes));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile userProfile = new UserProfile
            {
                User = user,
                TotalLikes = 5,
                TotalWatchTimeSeconds = 1800,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 5,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            };

            await repository.UpsertProfileAsync(userProfile);

            UserProfile? inserted = await context.UserProfiles.FirstOrDefaultAsync();

            Assert.Equal(5, inserted!.TotalLikes);
        }

        [Fact]
        public async Task UpsertProfileAsync_existingProfile_updatesTotalLikes()
        {
            await using AppDbContext context = CreateContext(nameof(UpsertProfileAsync_existingProfile_updatesTotalLikes));
            User user = await SeedUser(context);

            context.UserProfiles.Add(new UserProfile
            {
                User = user,
                TotalLikes = 5,
                TotalWatchTimeSeconds = 1800,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 5,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile updatedProfile = new UserProfile
            {
                User = user,
                TotalLikes = 20,
                TotalWatchTimeSeconds = 7200,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 20,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            };

            await repository.UpsertProfileAsync(updatedProfile);

            UserProfile? result = await context.UserProfiles
                .FirstOrDefaultAsync(existingProfile => existingProfile.User.Id == user.Id);

            Assert.Equal(20, result!.TotalLikes);
        }

        [Fact]
        public async Task UpsertProfileAsync_existingProfile_updatesTotalWatchTime()
        {
            await using AppDbContext context = CreateContext(nameof(UpsertProfileAsync_existingProfile_updatesTotalWatchTime));
            User user = await SeedUser(context);

            context.UserProfiles.Add(new UserProfile
            {
                User = user,
                TotalLikes = 5,
                TotalWatchTimeSeconds = 1800,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 5,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile updatedProfile = new UserProfile
            {
                User = user,
                TotalLikes = 20,
                TotalWatchTimeSeconds = 7200,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 20,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            };

            await repository.UpsertProfileAsync(updatedProfile);

            UserProfile? result = await context.UserProfiles
                .FirstOrDefaultAsync(existingProfile => existingProfile.User.Id == user.Id);

            Assert.Equal(7200, result!.TotalWatchTimeSeconds);
        }

        [Fact]
        public async Task UpsertProfileAsync_existingProfile_doesNotInsertDuplicate()
        {
            await using AppDbContext context = CreateContext(nameof(UpsertProfileAsync_existingProfile_doesNotInsertDuplicate));
            User user = await SeedUser(context);

            context.UserProfiles.Add(new UserProfile
            {
                User = user,
                TotalLikes = 5,
                TotalWatchTimeSeconds = 1800,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 5,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile updatedProfile = new UserProfile
            {
                User = user,
                TotalLikes = 20,
                TotalWatchTimeSeconds = 7200,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 20,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            };

            await repository.UpsertProfileAsync(updatedProfile);

            int count = await context.UserProfiles.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task BuildProfileFromInteractionsAsync_noInteractions_returnsZeroTotalLikes()
        {
            await using AppDbContext context = CreateContext(nameof(BuildProfileFromInteractionsAsync_noInteractions_returnsZeroTotalLikes));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile result = await repository.BuildProfileFromInteractionsAsync(user.Id);

            Assert.Equal(0, result.TotalLikes);
        }

        [Fact]
        public async Task BuildProfileFromInteractionsAsync_noInteractions_returnsZeroTotalClipsViewed()
        {
            await using AppDbContext context = CreateContext(nameof(BuildProfileFromInteractionsAsync_noInteractions_returnsZeroTotalClipsViewed));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile result = await repository.BuildProfileFromInteractionsAsync(user.Id);

            Assert.Equal(0, result.TotalClipsViewed);
        }

        [Fact]
        public async Task BuildProfileFromInteractionsAsync_noInteractions_returnsZeroLikeToViewRatio()
        {
            await using AppDbContext context = CreateContext(nameof(BuildProfileFromInteractionsAsync_noInteractions_returnsZeroLikeToViewRatio));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile result = await repository.BuildProfileFromInteractionsAsync(user.Id);

            Assert.Equal(0m, result.LikeToViewRatio);
        }

        [Fact]
        public async Task BuildProfileFromInteractionsAsync_noInteractions_returnsZeroTotalWatchTime()
        {
            await using AppDbContext context = CreateContext(nameof(BuildProfileFromInteractionsAsync_noInteractions_returnsZeroTotalWatchTime));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile result = await repository.BuildProfileFromInteractionsAsync(user.Id);

            Assert.Equal(0, result.TotalWatchTimeSeconds);
        }

        [Fact]
        public async Task BuildProfileFromInteractionsAsync_withInteractions_returnsCorrectTotalClipsViewed()
        {
            await using AppDbContext context = CreateContext(nameof(BuildProfileFromInteractionsAsync_withInteractions_returnsCorrectTotalClipsViewed));
            User user = await SeedUser(context);

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

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = true,
                WatchDurationSeconds = 20m,
                WatchPercentage = 66m,
                ViewedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile result = await repository.BuildProfileFromInteractionsAsync(user.Id);

            Assert.Equal(1, result.TotalClipsViewed);
        }

        [Fact]
        public async Task BuildProfileFromInteractionsAsync_withLikedInteraction_returnsCorrectTotalLikes()
        {
            await using AppDbContext context = CreateContext(nameof(BuildProfileFromInteractionsAsync_withLikedInteraction_returnsCorrectTotalLikes));
            User user = await SeedUser(context);

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

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = true,
                WatchDurationSeconds = 20m,
                WatchPercentage = 66m,
                ViewedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile result = await repository.BuildProfileFromInteractionsAsync(user.Id);

            Assert.Equal(1, result.TotalLikes);
        }
    }
}
