using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;
using System.Threading.Tasks;
using System;

namespace MovieApp.Tests.Repositories
{
    public class InteractionRepositoryTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        private static async Task<(User user, Reel reel)> SeedUserAndReel(AppDbContext context)
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

            return (user, reel);
        }

        [Fact]
        public async Task GetInteractionAsync_noInteraction_returnsNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetInteractionAsync_noInteraction_returnsNull));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            UserReelInteraction? result = await repository.GetInteractionAsync(user.Id, reel.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task InsertInteractionAsync_validInput_insertsInteraction()
        {
            await using AppDbContext context = CreateContext(nameof(InsertInteractionAsync_validInput_insertsInteraction));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            UserReelInteraction interaction = new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = true,
                WatchDurationSeconds = 20m,
                WatchPercentage = 66m,
                ViewedAt = DateTime.UtcNow,
            };

            await repository.InsertInteractionAsync(interaction);

            int count = await context.UserReelInteractions.CountAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task UpsertInteractionAsync_noExistingInteraction_insertsInteraction()
        {
            await using AppDbContext context = CreateContext(nameof(UpsertInteractionAsync_noExistingInteraction_insertsInteraction));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpsertInteractionAsync(user.Id, reel.Id);

            int count = await context.UserReelInteractions.CountAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task UpsertInteractionAsync_existingInteraction_doesNotInsertDuplicate()
        {
            await using AppDbContext context = CreateContext(nameof(UpsertInteractionAsync_existingInteraction_doesNotInsertDuplicate));
            (User user, Reel reel) = await SeedUserAndReel(context);

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = false,
                WatchDurationSeconds = 0m,
                WatchPercentage = 0m,
                ViewedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpsertInteractionAsync(user.Id, reel.Id);

            int count = await context.UserReelInteractions.CountAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task ToggleLikeAsync_noExistingInteraction_createsLikedInteraction()
        {
            await using AppDbContext context = CreateContext(nameof(ToggleLikeAsync_noExistingInteraction_createsLikedInteraction));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            await repository.ToggleLikeAsync(user.Id, reel.Id);

            UserReelInteraction? interaction = await context.UserReelInteractions
                .FirstOrDefaultAsync(i => i.User.Id == user.Id && i.Reel.Id == reel.Id);

            Assert.NotNull(interaction);
            Assert.True(interaction.IsLiked);
        }

        [Fact]
        public async Task ToggleLikeAsync_existingLikedInteraction_setsIsLikedToFalse()
        {
            await using AppDbContext context = CreateContext(nameof(ToggleLikeAsync_existingLikedInteraction_setsIsLikedToFalse));
            (User user, Reel reel) = await SeedUserAndReel(context);

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = true,
                WatchDurationSeconds = 0m,
                WatchPercentage = 0m,
                ViewedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            InteractionRepository repository = new InteractionRepository(context);

            await repository.ToggleLikeAsync(user.Id, reel.Id);

            UserReelInteraction? interaction = await context.UserReelInteractions
                .FirstOrDefaultAsync(i => i.User.Id == user.Id && i.Reel.Id == reel.Id);

            Assert.NotNull(interaction);
            Assert.False(interaction.IsLiked);
        }

        [Fact]
        public async Task GetLikeCountAsync_multipleLikes_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext(nameof(GetLikeCountAsync_multipleLikes_returnsCorrectCount));
            (User user, Reel reel) = await SeedUserAndReel(context);

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = true,
                WatchDurationSeconds = 0m,
                WatchPercentage = 0m,
                ViewedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            InteractionRepository repository = new InteractionRepository(context);

            int count = await repository.GetLikeCountAsync(reel.Id);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetReelMovieIdAsync_reelExists_returnsMovieId()
        {
            await using AppDbContext context = CreateContext(nameof(GetReelMovieIdAsync_reelExists_returnsMovieId));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            int? movieId = await repository.GetReelMovieIdAsync(reel.Id);

            Assert.NotNull(movieId);
            Assert.Equal(reel.Movie.Id, movieId);
        }

        [Fact]
        public async Task GetReelMovieIdAsync_reelDoesNotExist_returnsNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetReelMovieIdAsync_reelDoesNotExist_returnsNull));

            InteractionRepository repository = new InteractionRepository(context);

            int? movieId = await repository.GetReelMovieIdAsync(999);

            Assert.Null(movieId);
        }
    }
}
