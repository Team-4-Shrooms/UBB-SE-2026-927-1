using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using System.Threading.Tasks;
using System;

namespace MovieApp.DataLayer.Repositories
{
    /// <summary>
    /// EF Core data access for the UserReelInteraction table.
    /// </summary>
    public class InteractionRepository : IInteractionRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public InteractionRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task InsertInteractionAsync(UserReelInteraction interaction)
        {
            _context.UserReelInteractions.Add(interaction);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpsertInteractionAsync(int userId, int reelId)
        {
            bool exists = await _context.UserReelInteractions
                .AnyAsync(interaction => interaction.User.Id == userId && interaction.Reel.Id == reelId);

            if (exists)
            {
                return;
            }

            User user = await _context.Users.FindAsync(userId)
                ?? throw new InvalidOperationException($"User {userId} not found.");
            Reel reel = await _context.Reels.FindAsync(reelId)
                ?? throw new InvalidOperationException($"Reel {reelId} not found.");

            UserReelInteraction interaction = new UserReelInteraction
            {
                User = user,
                Reel = reel,
                IsLiked = false,
                WatchDurationSeconds = 0m,
                WatchPercentage = 0m,
                ViewedAt = DateTime.UtcNow,
            };

            _context.UserReelInteractions.Add(interaction);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task ToggleLikeAsync(int userId, int reelId)
        {
            UserReelInteraction? existingInteraction = await GetInteractionAsync(userId, reelId);

            if (existingInteraction is null)
            {
                User user = await _context.Users.FindAsync(userId)
                    ?? throw new InvalidOperationException($"User {userId} not found.");
                Reel reel = await _context.Reels.FindAsync(reelId)
                    ?? throw new InvalidOperationException($"Reel {reelId} not found.");

                UserReelInteraction newInteraction = new UserReelInteraction
                {
                    User = user,
                    Reel = reel,
                    IsLiked = true,
                    WatchDurationSeconds = 0m,
                    WatchPercentage = 0m,
                    ViewedAt = DateTime.UtcNow,
                };

                _context.UserReelInteractions.Add(newInteraction);
            }
            else
            {
                existingInteraction.IsLiked = !existingInteraction.IsLiked;
            }

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateViewDataAsync(int userId, int reelId, decimal watchDurationSeconds, decimal watchPercentage)
        {
            UserReelInteraction? existingInteraction = await GetInteractionAsync(userId, reelId);

            if (existingInteraction is null)
            {
                User user = await _context.Users.FindAsync(userId)
                    ?? throw new InvalidOperationException($"User {userId} not found.");
                Reel reel = await _context.Reels.FindAsync(reelId)
                    ?? throw new InvalidOperationException($"Reel {reelId} not found.");

                UserReelInteraction newInteraction = new UserReelInteraction
                {
                    User = user,
                    Reel = reel,
                    IsLiked = false,
                    WatchDurationSeconds = watchDurationSeconds,
                    WatchPercentage = watchPercentage,
                    ViewedAt = DateTime.UtcNow,
                };

                _context.UserReelInteractions.Add(newInteraction);
            }
            else
            {
                existingInteraction.WatchDurationSeconds = watchDurationSeconds;
                existingInteraction.WatchPercentage = watchPercentage;
                existingInteraction.ViewedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId)
        {
            return await _context.UserReelInteractions
                .Include(interaction => interaction.User)
                .Include(interaction => interaction.Reel)
                .FirstOrDefaultAsync(interaction => interaction.User.Id == userId && interaction.Reel.Id == reelId);
        }

        /// <inheritdoc />
        public async Task<int> GetLikeCountAsync(int reelId)
        {
            return await _context.UserReelInteractions
                .CountAsync(interaction => interaction.Reel.Id == reelId && interaction.IsLiked);
        }

        /// <inheritdoc />
        public async Task<int?> GetReelMovieIdAsync(int reelId)
        {
            Reel? reel = await _context.Reels
                .Include(currentReel => currentReel.Movie)
                .FirstOrDefaultAsync(currentReel => currentReel.Id == reelId);

            return reel?.Movie.Id;
        }
    }
}
