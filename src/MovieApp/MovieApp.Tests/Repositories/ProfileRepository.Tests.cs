using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
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
        public async Task GetProfileAsync_ProfileDoesNotExist_ReturnsNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_ProfileDoesNotExist_ReturnsNull));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile? foundProfile = await repository.GetProfileAsync(user.Id);

            Assert.Null(foundProfile);
        }

        [Fact]
        public async Task GetProfileAsync_ProfileExists_ReturnsNotNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_ProfileExists_ReturnsNotNull));
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

            UserProfile? foundProfile = await repository.GetProfileAsync(user.Id);

            Assert.NotNull(foundProfile);
        }

        [Fact]
        public async Task GetProfileAsync_ProfileExists_ReturnsCorrectTotalLikes()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_ProfileExists_ReturnsCorrectTotalLikes));
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

            UserProfile? foundProfile = await repository.GetProfileAsync(user.Id);

            Assert.Equal(10, foundProfile!.TotalLikes);
        }

        [Fact]
        public async Task GetProfileAsync_ProfileExists_ReturnsCorrectTotalWatchTime()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_ProfileExists_ReturnsCorrectTotalWatchTime));
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

            UserProfile? foundProfile = await repository.GetProfileAsync(user.Id);

            Assert.Equal(3600, foundProfile!.TotalWatchTimeSeconds);
        }

        [Fact]
        public async Task GetProfileAsync_WrongUserId_ReturnsNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_WrongUserId_ReturnsNull));
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

            UserProfile? foundProfile = await repository.GetProfileAsync(999);

            Assert.Null(foundProfile);
        }

        [Fact]
        public async Task GetInteractionsAsync_NoInteractionsExist_ReturnsEmptyList()
        {
            await using AppDbContext context = CreateContext(nameof(GetInteractionsAsync_NoInteractionsExist_ReturnsEmptyList));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            List<UserReelInteraction> interactions = await repository.GetInteractionsAsync(user.Id);

            Assert.Empty(interactions);
        }

        [Fact]
        public async Task GetInteractionsAsync_OneInteractionExists_ReturnsSingleInteraction()
        {
            await using AppDbContext context = CreateContext(nameof(GetInteractionsAsync_OneInteractionExists_ReturnsSingleInteraction));
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

            List<UserReelInteraction> interactions = await repository.GetInteractionsAsync(user.Id);

            Assert.Single(interactions);
        }

        [Fact]
        public async Task AddProfileAsync_ValidProfile_CreatesOneRecord()
        {
            await using AppDbContext context = CreateContext(nameof(AddProfileAsync_ValidProfile_CreatesOneRecord));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile profile = new UserProfile
            {
                User = user,
                TotalLikes = 5,
                TotalWatchTimeSeconds = 1800,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 5,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            };

            await repository.AddProfileAsync(profile);
            await repository.SaveChangesAsync();

            int profileCount = await context.UserProfiles.CountAsync();

            Assert.Equal(1, profileCount);
        }

        [Fact]
        public async Task AddProfileAsync_ValidProfile_StoresCorrectTotalLikes()
        {
            await using AppDbContext context = CreateContext(nameof(AddProfileAsync_ValidProfile_StoresCorrectTotalLikes));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile profile = new UserProfile
            {
                User = user,
                TotalLikes = 5,
                TotalWatchTimeSeconds = 1800,
                AverageWatchTimeSeconds = 360m,
                TotalClipsViewed = 5,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            };

            await repository.AddProfileAsync(profile);
            await repository.SaveChangesAsync();

            UserProfile? insertedProfile = await context.UserProfiles.FirstOrDefaultAsync();

            Assert.Equal(5, insertedProfile!.TotalLikes);
        }
    }
}
