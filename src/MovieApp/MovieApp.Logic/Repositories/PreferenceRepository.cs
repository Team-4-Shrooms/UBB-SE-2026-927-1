using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;
using System.Threading.Tasks;
using System;

namespace MovieApp.Logic.Repositories
{
    /// <summary>
    /// EF Core data access for the UserMoviePreference table.
    /// </summary>
    public class PreferenceRepository : IPreferenceRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreferenceRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public PreferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<bool> PreferenceExistsAsync(int userId, int movieId)
        {
            return await _context.UserMoviePreferences
                .AnyAsync(preference => preference.User.Id == userId && preference.Movie.Id == movieId);
        }

        /// <inheritdoc />
        public async Task InsertPreferenceAsync(int userId, int movieId, decimal score)
        {
            UserMoviePreference preference = new UserMoviePreference
            {
                User = await _context.Users.FindAsync(userId) ?? throw new InvalidOperationException($"User {userId} not found."),
                Movie = await _context.Movies.FindAsync(movieId) ?? throw new InvalidOperationException($"Movie {movieId} not found."),
                Score = score,
                LastModified = DateTime.UtcNow,
                ChangeFromPreviousValue = 0,
            };

            _context.UserMoviePreferences.Add(preference);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdatePreferenceAsync(int userId, int movieId, decimal boost)
        {
            UserMoviePreference? preference = await _context.UserMoviePreferences
                .FirstOrDefaultAsync(currentPreference => currentPreference.User.Id == userId && currentPreference.Movie.Id == movieId);

            if (preference is null)
            {
                return;
            }

            preference.Score += boost;
            preference.LastModified = DateTime.UtcNow;
            preference.ChangeFromPreviousValue = (int)boost;

            await _context.SaveChangesAsync();
        }
    }
}
