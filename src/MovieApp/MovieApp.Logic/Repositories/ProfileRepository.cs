using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MovieApp.Logic.Repositories
{
    /// <summary>
    /// EF Core data access for the UserProfile table.
    /// </summary>
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<UserProfile?> GetProfileAsync(int userId)
        {
            return await _context.UserProfiles
                .FirstOrDefaultAsync(profile => profile.User.Id == userId);
        }

        /// <inheritdoc />
        public async Task<UserProfile> BuildProfileFromInteractionsAsync(int userId)
        {
            List<UserReelInteraction> interactions = await _context.UserReelInteractions
                .Where(interaction => interaction.User.Id == userId)
                .ToListAsync();

            int totalLikes = interactions.Count(interaction => interaction.IsLiked);
            long totalWatchTimeSeconds = (long)interactions.Sum(interaction => interaction.WatchDurationSeconds);
            decimal averageWatchTimeSeconds = interactions.Count > 0
                ? interactions.Average(interaction => interaction.WatchDurationSeconds)
                : 0m;
            int totalClipsViewed = interactions.Count;
            decimal likeToViewRatio = CalculateLikeToViewRatio(totalLikes, totalClipsViewed);

            return new UserProfile
            {
                User = await _context.Users.FindAsync(userId) ?? throw new InvalidOperationException($"User {userId} not found."),
                TotalLikes = totalLikes,
                TotalWatchTimeSeconds = totalWatchTimeSeconds,
                AverageWatchTimeSeconds = averageWatchTimeSeconds,
                TotalClipsViewed = totalClipsViewed,
                LikeToViewRatio = likeToViewRatio,
                LastUpdated = DateTime.UtcNow,
            };
        }

        /// <inheritdoc />
        public async Task UpsertProfileAsync(UserProfile profile)
        {
            UserProfile? existingProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.User.Id == profile.User.Id);

            if (existingProfile is null)
            {
                _context.UserProfiles.Add(profile);
            }
            else
            {
                existingProfile.TotalLikes = profile.TotalLikes;
                existingProfile.TotalWatchTimeSeconds = profile.TotalWatchTimeSeconds;
                existingProfile.AverageWatchTimeSeconds = profile.AverageWatchTimeSeconds;
                existingProfile.TotalClipsViewed = profile.TotalClipsViewed;
                existingProfile.LikeToViewRatio = profile.LikeToViewRatio;
                existingProfile.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Calculates the like-to-view ratio based on total likes and total clips viewed.
        /// </summary>
        /// <param name="totalLikes">Total number of likes.</param>
        /// <param name="totalClipsViewed">Total number of clips viewed.</param>
        /// <returns>The ratio of likes to views, or 0 if no clips have been viewed.</returns>
        private static decimal CalculateLikeToViewRatio(int totalLikes, int totalClipsViewed)
        {
            return totalClipsViewed == 0 ? 0m : (decimal)totalLikes / totalClipsViewed;
        }
    }
}
