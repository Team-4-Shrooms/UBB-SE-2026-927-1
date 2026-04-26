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
        public async Task GetUserReelsAsync_reelsExist_returnsReelsForUser()
        {
            await using AppDbContext context = CreateContext(nameof(GetUserReelsAsync_reelsExist_returnsReelsForUser));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            IList<Reel> result = await repository.GetUserReelsAsync(user.Id);

            Assert.Single(result);
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
        public async Task GetReelByIdAsync_reelExists_returnsReel()
        {
            await using AppDbContext context = CreateContext(nameof(GetReelByIdAsync_reelExists_returnsReel));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            Reel? result = await repository.GetReelByIdAsync(reel.Id);

            Assert.NotNull(result);
            Assert.Equal(reel.Title, result.Title);
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
        public async Task UpdateReelEditsAsync_reelExists_updatesCropData()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateReelEditsAsync_reelExists_updatesCropData));
            (User user, Movie movie, Reel reel) = await SeedData(context);

            ReelRepository repository = new ReelRepository(context);

            await repository.UpdateReelEditsAsync(reel.Id, "{crop:true}", null, string.Empty);

            Reel? updated = await context.Reels.FindAsync(reel.Id);
            Assert.Equal("{crop:true}", updated!.CropDataJson);
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
    }
}
