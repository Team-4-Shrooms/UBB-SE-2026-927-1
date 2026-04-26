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
        public async Task GetProfileAsync_profileExists_returnsProfile()
        {
            await using AppDbContext context = CreateContext(nameof(GetProfileAsync_profileExists_returnsProfile));
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
            Assert.Equal(10, result.TotalLikes);
        }

        [Fact]
        public async Task UpsertProfileAsync_noExistingProfile_insertsProfile()
        {
            await using AppDbContext context = CreateContext(nameof(UpsertProfileAsync_noExistingProfile_insertsProfile));
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

            await repository.UpsertProfileAsync(profile);

            int count = await context.UserProfiles.CountAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task UpsertProfileAsync_existingProfile_updatesProfile()
        {
            await using AppDbContext context = CreateContext(nameof(UpsertProfileAsync_existingProfile_updatesProfile));
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
                .FirstOrDefaultAsync(p => p.User.Id == user.Id);

            Assert.NotNull(result);
            Assert.Equal(20, result.TotalLikes);
            Assert.Equal(7200, result.TotalWatchTimeSeconds);
        }

        [Fact]
        public async Task BuildProfileFromInteractionsAsync_noInteractions_returnsZeroProfile()
        {
            await using AppDbContext context = CreateContext(nameof(BuildProfileFromInteractionsAsync_noInteractions_returnsZeroProfile));
            User user = await SeedUser(context);

            ProfileRepository repository = new ProfileRepository(context);

            UserProfile result = await repository.BuildProfileFromInteractionsAsync(user.Id);

            Assert.Equal(0, result.TotalLikes);
            Assert.Equal(0, result.TotalClipsViewed);
            Assert.Equal(0m, result.LikeToViewRatio);
        }
    }
}
