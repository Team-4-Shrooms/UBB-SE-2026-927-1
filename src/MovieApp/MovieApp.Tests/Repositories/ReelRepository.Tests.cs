using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MovieApp.Tests.Repositories
{
    public class ReelRepositoryTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        private static async Task<(User user, Movie movie, Reel reel)> SeedData(AppDbContext context)
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
        public async Task GetUserReelsAsync_reelsExist_returnsSingleReel()
        {
            await using AppDbContext context = CreateContext(nameof(GetUserReelsAsync_reelsExist_returnsSingleReel));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            IList<Reel> result = await repository.GetUserReelsAsync(user.Id);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetUserReelsAsync_reelsExist_returnsCorrectReelId()
        {
            await using AppDbContext context = CreateContext(nameof(GetUserReelsAsync_reelsExist_returnsCorrectReelId));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            IList<Reel> result = await repository.GetUserReelsAsync(user.Id);

            Assert.Equal(reel.Id, result[0].Id);
        }

        [Fact]
        public async Task GetUserReelsAsync_noReels_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetUserReelsAsync_noReels_returnsEmpty));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            IList<Reel> result = await repository.GetUserReelsAsync(999);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetReelByIdAsync_reelExists_returnsNotNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetReelByIdAsync_reelExists_returnsNotNull));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            Reel? result = await repository.GetReelByIdAsync(reel.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetReelByIdAsync_reelExists_returnsCorrectTitle()
        {
            await using AppDbContext context = CreateContext(nameof(GetReelByIdAsync_reelExists_returnsCorrectTitle));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            Reel? result = await repository.GetReelByIdAsync(reel.Id);

            Assert.Equal(reel.Title, result!.Title);
        }

        [Fact]
        public async Task GetReelByIdAsync_reelExists_returnsCorrectVideoUrl()
        {
            await using AppDbContext context = CreateContext(nameof(GetReelByIdAsync_reelExists_returnsCorrectVideoUrl));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            Reel? result = await repository.GetReelByIdAsync(reel.Id);

            Assert.Equal(reel.VideoUrl, result!.VideoUrl);
        }

        [Fact]
        public async Task GetReelByIdAsync_reelDoesNotExist_returnsNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetReelByIdAsync_reelDoesNotExist_returnsNull));

            ReelRepository repository = new ReelRepository(context);

            Reel? result = await repository.GetReelByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateReelEditsAsync_reelExists_updatesCropDataJson()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateReelEditsAsync_reelExists_updatesCropDataJson));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            await repository.UpdateReelEditsAsync(reel.Id, "{crop:true}", null, string.Empty);

            Reel? updatedReel = await context.Reels.FindAsync(reel.Id);

            Assert.Equal("{crop:true}", updatedReel!.CropDataJson);
        }

        [Fact]
        public async Task UpdateReelEditsAsync_nonExistentReel_doesNotThrow()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateReelEditsAsync_nonExistentReel_doesNotThrow));

            ReelRepository repository = new ReelRepository(context);

            await repository.UpdateReelEditsAsync(999, "{crop:true}", null, string.Empty);

            int count = await context.Reels.CountAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task DeleteReelAsync_reelExists_removesReel()
        {
            await using AppDbContext context = CreateContext(nameof(DeleteReelAsync_reelExists_removesReel));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            await repository.DeleteReelAsync(reel.Id);

            int count = await context.Reels.CountAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task DeleteReelAsync_reelDoesNotExist_doesNotThrow()
        {
            await using AppDbContext context = CreateContext(nameof(DeleteReelAsync_reelDoesNotExist_doesNotThrow));

            ReelRepository repository = new ReelRepository(context);

            await repository.DeleteReelAsync(999);

            int count = await context.Reels.CountAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task DeleteReelAsync_reelExists_doesNotDeleteOtherReels()
        {
            await using AppDbContext context = CreateContext(nameof(DeleteReelAsync_reelExists_doesNotDeleteOtherReels));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            Reel secondReel = new Reel
            {
                VideoUrl = "http://second.url",
                ThumbnailUrl = "http://thumb2.url",
                Title = "Second Reel",
                Caption = "caption2",
                FeatureDurationSeconds = 20m,
                Source = "upload",
                CreatedAt = DateTime.UtcNow,
                Movie = movie,
                CreatorUser = user,
            };
            context.Reels.Add(secondReel);
            await context.SaveChangesAsync();

            ReelRepository repository = new ReelRepository(context);

            await repository.DeleteReelAsync(reel.Id);

            int count = await context.Reels.CountAsync();

            Assert.Equal(1, count);
        }
    }
}
