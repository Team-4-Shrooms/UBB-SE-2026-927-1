using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MovieApp.DataLayer.Repositories
{
    /// <summary>
    /// EF Core data access for recommendation inputs.
    /// </summary>
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public RecommendationRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
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
                .Include(reel => reel.CreatorUser)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Dictionary<int, decimal>> GetUserPreferenceScoresAsync(int userId)
        {
            var preferences = await _context.UserMoviePreferences
                .Where(preference => preference.User.Id == userId)
                .Select(preference => new { MovieId = preference.Movie.Id, preference.Score })
                .ToListAsync();

            // Use GroupBy to handle cases where multiple preferences might exist for the same movie
            return preferences
                .GroupBy(p => p.MovieId)
                .ToDictionary(
                    group => group.Key,
                    group => group.First().Score);
        }

        /// <inheritdoc />
        public async Task<Dictionary<int, int>> GetAllLikeCountsAsync()
        {
            List<UserReelInteraction> likedInteractions = await _context.UserReelInteractions
                .Include(interaction => interaction.Reel)
                .Where(interaction => interaction.IsLiked)
                .ToListAsync();                           

            return likedInteractions
                .GroupBy(interaction => interaction.Reel.Id)
                .ToDictionary(
                    group => group.Key,
                    group => group.Count());
        }

        /// <inheritdoc />
        public async Task<List<UserReelInteraction>> GetLikesWithinDaysAsync(int days)
        {
            DateTime cutoff = DateTime.UtcNow.AddDays(-days);

            return await _context.UserReelInteractions
                .Include(interaction => interaction.User)
                .Include(interaction => interaction.Reel)
                    .ThenInclude(reel => reel.Movie)
                .Where(interaction => interaction.IsLiked && interaction.ViewedAt >= cutoff)
                .ToListAsync();
        }
    }
}
