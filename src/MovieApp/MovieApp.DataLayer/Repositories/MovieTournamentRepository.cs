using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MovieApp.Logic.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IMovieTournamentRepository"/>.
    /// Responsible for querying and updating movie tournament data,
    /// including pool retrieval and score boosting.
    /// </summary>
    public class MovieTournamentRepository : IMovieTournamentRepository
    {
        private readonly MovieApp.Logic.Data.IMovieAppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieTournamentRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public MovieTournamentRepository(MovieApp.Logic.Data.IMovieAppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<int> GetTournamentPoolSizeAsync(int userId)
        {
            return await _context.UserMoviePreferences
                .CountAsync(preference =>
                    preference.User.Id == userId &&
                    preference.ChangeFromPreviousValue > 0);
        }

        /// <inheritdoc />
        public async Task<List<Movie>> GetTournamentPoolAsync(int userId, int poolSize)
        {
            return await _context.UserMoviePreferences
                .Where(preference =>
                    preference.User.Id == userId &&
                    preference.ChangeFromPreviousValue > 0)
                .OrderByDescending(preference => preference.LastModified)
                .Take(poolSize)
                .Select(preference => preference.Movie)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task BoostMovieScoreAsync(int userId, int movieId, decimal scoreBoost)
        {
            UserMoviePreference? preference = await _context.UserMoviePreferences
                .FirstOrDefaultAsync(currentPreference =>
                    currentPreference.User.Id == userId &&
                    currentPreference.Movie.Id == movieId);

            if (preference is null)
            {
                return;
            }

            preference.Score += scoreBoost;
            preference.LastModified = DateTime.UtcNow;
            preference.ChangeFromPreviousValue = (int)scoreBoost;

            await _context.SaveChangesAsync();
        }
    }
}
