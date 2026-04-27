using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;
using System.Threading.Tasks;
using System;

namespace MovieApp.Tests.Repositories
{
    public class VideoStorageRepositoryTests
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
        public async Task InsertReelAsync_validReel_returnsPositiveId()
        {
            await using AppDbContext context = CreateContext(nameof(InsertReelAsync_validReel_returnsPositiveId));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            Reel reel = new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "New Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "upload",
                Movie = movie,
                CreatorUser = user,
            };

            VideoStorageRepository repository = new VideoStorageRepository(context);

            Reel result = await repository.InsertReelAsync(reel);

            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task InsertReelAsync_validReel_insertsOneRecord()
        {
            await using AppDbContext context = CreateContext(nameof(InsertReelAsync_validReel_insertsOneRecord));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            Reel reel = new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "New Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "upload",
                Movie = movie,
                CreatorUser = user,
            };

            VideoStorageRepository repository = new VideoStorageRepository(context);

            await repository.InsertReelAsync(reel);

            int count = await context.Reels.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task InsertReelAsync_validReel_setsCreatedAtAfterMinValue()
        {
            await using AppDbContext context = CreateContext(nameof(InsertReelAsync_validReel_setsCreatedAtAfterMinValue));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            Reel reel = new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "New Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "upload",
                Movie = movie,
                CreatorUser = user,
            };

            VideoStorageRepository repository = new VideoStorageRepository(context);

            Reel result = await repository.InsertReelAsync(reel);

            Assert.True(result.CreatedAt > DateTime.MinValue);
        }

        [Fact]
        public async Task InsertReelAsync_validReel_setsCreatedAtToRecent()
        {
            await using AppDbContext context = CreateContext(nameof(InsertReelAsync_validReel_setsCreatedAtToRecent));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            DateTime beforeInsert = DateTime.UtcNow.AddSeconds(-1);

            Reel reel = new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "New Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "upload",
                Movie = movie,
                CreatorUser = user,
            };

            VideoStorageRepository repository = new VideoStorageRepository(context);

            Reel result = await repository.InsertReelAsync(reel);

            Assert.True(result.CreatedAt >= beforeInsert);
        }

        [Fact]
        public async Task InsertReelAsync_validReel_returnsCorrectTitle()
        {
            await using AppDbContext context = CreateContext(nameof(InsertReelAsync_validReel_returnsCorrectTitle));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            Reel reel = new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "My Special Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "upload",
                Movie = movie,
                CreatorUser = user,
            };

            VideoStorageRepository repository = new VideoStorageRepository(context);

            Reel result = await repository.InsertReelAsync(reel);

            Assert.Equal("My Special Reel", result.Title);
        }

        [Fact]
        public async Task InsertReelAsync_validReel_returnsCorrectVideoUrl()
        {
            await using AppDbContext context = CreateContext(nameof(InsertReelAsync_validReel_returnsCorrectVideoUrl));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            Reel reel = new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "New Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "upload",
                Movie = movie,
                CreatorUser = user,
            };

            VideoStorageRepository repository = new VideoStorageRepository(context);

            Reel result = await repository.InsertReelAsync(reel);

            Assert.Equal("http://video.url", result.VideoUrl);
        }

        [Fact]
        public async Task InsertReelAsync_validReel_returnsCorrectFeatureDuration()
        {
            await using AppDbContext context = CreateContext(nameof(InsertReelAsync_validReel_returnsCorrectFeatureDuration));
            (User user, Movie movie) = await SeedUserAndMovie(context);

            Reel reel = new Reel
            {
                VideoUrl = "http://video.url",
                ThumbnailUrl = "http://thumb.url",
                Title = "New Reel",
                Caption = "caption",
                FeatureDurationSeconds = 30m,
                Source = "upload",
                Movie = movie,
                CreatorUser = user,
            };

            VideoStorageRepository repository = new VideoStorageRepository(context);

            Reel result = await repository.InsertReelAsync(reel);

            Assert.Equal(30m, result.FeatureDurationSeconds);
        }
    }
}
