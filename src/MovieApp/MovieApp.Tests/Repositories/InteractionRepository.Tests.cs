using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
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
        public async Task GetInteractionAsync_interactionExists_returnsNotNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetInteractionAsync_interactionExists_returnsNotNull));
            (User user, Reel reel) = await SeedUserAndReel(context);

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

            InteractionRepository repository = new InteractionRepository(context);

            UserReelInteraction? result = await repository.GetInteractionAsync(user.Id, reel.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetInteractionAsync_interactionExists_returnsCorrectUserId()
        {
            await using AppDbContext context = CreateContext(nameof(GetInteractionAsync_interactionExists_returnsCorrectUserId));
            (User user, Reel reel) = await SeedUserAndReel(context);

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

            InteractionRepository repository = new InteractionRepository(context);

            UserReelInteraction? result = await repository.GetInteractionAsync(user.Id, reel.Id);

            Assert.Equal(user.Id, result!.User.Id);
        }

        [Fact]
        public async Task GetInteractionAsync_interactionExists_returnsCorrectReelId()
        {
            await using AppDbContext context = CreateContext(nameof(GetInteractionAsync_interactionExists_returnsCorrectReelId));
            (User user, Reel reel) = await SeedUserAndReel(context);

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

            InteractionRepository repository = new InteractionRepository(context);

            UserReelInteraction? result = await repository.GetInteractionAsync(user.Id, reel.Id);

            Assert.Equal(reel.Id, result!.Reel.Id);
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
        public async Task InsertInteractionAsync_validInput_preservesIsLiked()
        {
            await using AppDbContext context = CreateContext(nameof(InsertInteractionAsync_validInput_preservesIsLiked));
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

            UserReelInteraction? inserted = await context.UserReelInteractions.FirstOrDefaultAsync();

            Assert.True(inserted!.IsLiked);
        }

        [Fact]
        public async Task InsertInteractionAsync_validInput_preservesWatchDuration()
        {
            await using AppDbContext context = CreateContext(nameof(InsertInteractionAsync_validInput_preservesWatchDuration));
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

            UserReelInteraction? inserted = await context.UserReelInteractions.FirstOrDefaultAsync();

            Assert.Equal(20m, inserted!.WatchDurationSeconds);
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
        public async Task UpsertInteractionAsync_noExistingInteraction_setsIsLikedToFalse()
        {
            await using AppDbContext context = CreateContext(nameof(UpsertInteractionAsync_noExistingInteraction_setsIsLikedToFalse));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpsertInteractionAsync(user.Id, reel.Id);

            UserReelInteraction? interaction = await context.UserReelInteractions.FirstOrDefaultAsync();

            Assert.False(interaction!.IsLiked);
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
        public async Task ToggleLikeAsync_noExistingInteraction_createsInteraction()
        {
            await using AppDbContext context = CreateContext(nameof(ToggleLikeAsync_noExistingInteraction_createsInteraction));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            await repository.ToggleLikeAsync(user.Id, reel.Id);

            int count = await context.UserReelInteractions.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task ToggleLikeAsync_noExistingInteraction_setsIsLikedToTrue()
        {
            await using AppDbContext context = CreateContext(nameof(ToggleLikeAsync_noExistingInteraction_setsIsLikedToTrue));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            await repository.ToggleLikeAsync(user.Id, reel.Id);

            UserReelInteraction? interaction = await context.UserReelInteractions
                .FirstOrDefaultAsync(existingInteraction => existingInteraction.User.Id == user.Id && existingInteraction.Reel.Id == reel.Id);

            Assert.True(interaction!.IsLiked);
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
                .FirstOrDefaultAsync(existingInteraction => existingInteraction.User.Id == user.Id && existingInteraction.Reel.Id == reel.Id);

            Assert.False(interaction!.IsLiked);
        }

        [Fact]
        public async Task ToggleLikeAsync_existingUnlikedInteraction_setsIsLikedToTrue()
        {
            await using AppDbContext context = CreateContext(nameof(ToggleLikeAsync_existingUnlikedInteraction_setsIsLikedToTrue));
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

            await repository.ToggleLikeAsync(user.Id, reel.Id);

            UserReelInteraction? interaction = await context.UserReelInteractions
                .FirstOrDefaultAsync(existingInteraction => existingInteraction.User.Id == user.Id && existingInteraction.Reel.Id == reel.Id);

            Assert.True(interaction!.IsLiked);
        }

        [Fact]
        public async Task GetLikeCountAsync_noLikes_returnsZero()
        {
            await using AppDbContext context = CreateContext(nameof(GetLikeCountAsync_noLikes_returnsZero));
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

            int count = await repository.GetLikeCountAsync(reel.Id);

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task GetLikeCountAsync_oneLike_returnsOne()
        {
            await using AppDbContext context = CreateContext(nameof(GetLikeCountAsync_oneLike_returnsOne));
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
        public async Task GetLikeCountAsync_nonExistentReel_returnsZero()
        {
            await using AppDbContext context = CreateContext(nameof(GetLikeCountAsync_nonExistentReel_returnsZero));

            InteractionRepository repository = new InteractionRepository(context);

            int count = await repository.GetLikeCountAsync(999);

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task GetReelMovieIdAsync_reelExists_returnsNotNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetReelMovieIdAsync_reelExists_returnsNotNull));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            int? movieId = await repository.GetReelMovieIdAsync(reel.Id);

            Assert.NotNull(movieId);
        }

        [Fact]
        public async Task GetReelMovieIdAsync_reelExists_returnsCorrectMovieId()
        {
            await using AppDbContext context = CreateContext(nameof(GetReelMovieIdAsync_reelExists_returnsCorrectMovieId));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            int? movieId = await repository.GetReelMovieIdAsync(reel.Id);

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

        [Fact]
        public async Task UpdateViewDataAsync_noExistingInteraction_insertsNewInteraction()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateViewDataAsync_noExistingInteraction_insertsNewInteraction));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpdateViewDataAsync(user.Id, reel.Id, 25m, 80m);

            int count = await context.UserReelInteractions.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task UpdateViewDataAsync_noExistingInteraction_storesCorrectWatchDuration()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateViewDataAsync_noExistingInteraction_storesCorrectWatchDuration));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpdateViewDataAsync(user.Id, reel.Id, 25m, 80m);

            UserReelInteraction? interaction = await context.UserReelInteractions.FirstOrDefaultAsync();

            Assert.Equal(25m, interaction!.WatchDurationSeconds);
        }

        [Fact]
        public async Task UpdateViewDataAsync_noExistingInteraction_storesCorrectWatchPercentage()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateViewDataAsync_noExistingInteraction_storesCorrectWatchPercentage));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpdateViewDataAsync(user.Id, reel.Id, 25m, 80m);

            UserReelInteraction? interaction = await context.UserReelInteractions.FirstOrDefaultAsync();

            Assert.Equal(80m, interaction!.WatchPercentage);
        }

        [Fact]
        public async Task UpdateViewDataAsync_noExistingInteraction_setsIsLikedToFalse()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateViewDataAsync_noExistingInteraction_setsIsLikedToFalse));
            (User user, Reel reel) = await SeedUserAndReel(context);

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpdateViewDataAsync(user.Id, reel.Id, 25m, 80m);

            UserReelInteraction? interaction = await context.UserReelInteractions.FirstOrDefaultAsync();

            Assert.False(interaction!.IsLiked);
        }

        [Fact]
        public async Task UpdateViewDataAsync_existingInteraction_updatesWatchDuration()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateViewDataAsync_existingInteraction_updatesWatchDuration));
            (User user, Reel reel) = await SeedUserAndReel(context);

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

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpdateViewDataAsync(user.Id, reel.Id, 25m, 80m);

            UserReelInteraction? interaction = await context.UserReelInteractions.FirstOrDefaultAsync();

            Assert.Equal(25m, interaction!.WatchDurationSeconds);
        }

        [Fact]
        public async Task UpdateViewDataAsync_existingInteraction_updatesWatchPercentage()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateViewDataAsync_existingInteraction_updatesWatchPercentage));
            (User user, Reel reel) = await SeedUserAndReel(context);

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

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpdateViewDataAsync(user.Id, reel.Id, 25m, 80m);

            UserReelInteraction? interaction = await context.UserReelInteractions.FirstOrDefaultAsync();

            Assert.Equal(80m, interaction!.WatchPercentage);
        }

        [Fact]
        public async Task UpdateViewDataAsync_existingInteraction_doesNotInsertDuplicate()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateViewDataAsync_existingInteraction_doesNotInsertDuplicate));
            (User user, Reel reel) = await SeedUserAndReel(context);

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

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpdateViewDataAsync(user.Id, reel.Id, 25m, 80m);

            int count = await context.UserReelInteractions.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task UpdateViewDataAsync_existingInteraction_updatesViewedAt()
        {
            await using AppDbContext context = CreateContext(nameof(UpdateViewDataAsync_existingInteraction_updatesViewedAt));
            (User user, Reel reel) = await SeedUserAndReel(context);

            context.UserReelInteractions.Add(new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = false,
                WatchDurationSeconds = 10m,
                WatchPercentage = 33m,
                ViewedAt = DateTime.UtcNow.AddDays(-1),
            });
            await context.SaveChangesAsync();

            DateTime beforeUpdate = DateTime.UtcNow.AddSeconds(-1);

            InteractionRepository repository = new InteractionRepository(context);

            await repository.UpdateViewDataAsync(user.Id, reel.Id, 25m, 80m);

            UserReelInteraction? interaction = await context.UserReelInteractions.FirstOrDefaultAsync();

            Assert.True(interaction!.ViewedAt >= beforeUpdate);
        }
    }
}
