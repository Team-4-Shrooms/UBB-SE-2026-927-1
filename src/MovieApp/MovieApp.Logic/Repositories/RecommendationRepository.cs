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
    /// EF Core data access for recommendation inputs.
    /// </summary>
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public RecommendationRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<bool> UserHasPreferencesAsync(int userId)
        {
            return await _context.UserMoviePreferences
                .AnyAsync(preference => preference.User.Id == userId);
        }

        /// <inheritdoc />
        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            return await _context.Reels
                .Include(reel => reel.Movie)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Dictionary<int, decimal>> GetUserPreferenceScoresAsync(int userId)
        {
            return await _context.UserMoviePreferences
                .Where(preference => preference.User.Id == userId)
                .ToDictionaryAsync(
                    preference => preference.Movie.Id,
                    preference => preference.Score);
        }

        /// <inheritdoc />
        public async Task<Dictionary<int, int>> GetAllLikeCountsAsync()
        {
            return await _context.UserReelInteractions
                .Where(interaction => interaction.IsLiked)
                .GroupBy(interaction => interaction.Reel.Id)
                .ToDictionaryAsync(
                    group => group.Key,
                    group => group.Count());
        }

        /// <inheritdoc />
        public async Task<List<UserReelInteraction>> GetLikesWithinDaysAsync(int days)
        {
            DateTime cutoff = DateTime.UtcNow.AddDays(-days);

            return await _context.UserReelInteractions
                .Where(interaction => interaction.IsLiked && interaction.ViewedAt >= cutoff)
                .ToListAsync();
        }
    }
}
