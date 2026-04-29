using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.DataLayer.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IPersonalityMatchRepository"/>.
    /// Provides data access for user movie preferences, user profiles,
    /// and related personality match operations.
    /// </summary>
    public class PersonalityMatchRepository : IPersonalityMatchRepository
    {
        private const string FallbackUsernamePrefix = "User";

        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonalityMatchRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public PersonalityMatchRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserMoviePreference>> GetAllPreferencesExceptUserAsync(int excludedUserId)
        {
            return await _context.UserMoviePreferences
                .Include(preferences => preferences.User)
                .Include(preferences => preferences.Movie)
                .Where(preferences => preferences.User.Id != excludedUserId)
                .ToListAsync();
        }

        public async Task<List<UserMoviePreference>> GetCurrentUserPreferencesAsync(int userId)
        {
            return await _context.UserMoviePreferences
                .Include(preferences => preferences.Movie)
                .Where(preferences => preferences.User.Id == userId)
                .ToListAsync();
        }

        public async Task<UserProfile?> GetUserProfileAsync(int userId)
        {
            return await _context.UserProfiles
                .Include(preferences => preferences.User)
                .FirstOrDefaultAsync(preferences => preferences.User.Id == userId);
        }

        public async Task<List<int>> GetRandomUserIdsAsync(int excludedUserId, int userIdsCount)
        {
            return await _context.UserMoviePreferences
                .Where(preferences => preferences.User.Id != excludedUserId)
                .Select(preferences => preferences.User.Id)
                .Distinct()
                .OrderBy(id => EF.Functions.Random())
                .Take(userIdsCount)
                .ToListAsync();
        }

        public async Task<List<MoviePreferenceDisplay>> GetTopPreferencesWithTitlesAsync(int userId, int count)
        {
            return await _context.UserMoviePreferences
                .Where(preferences => preferences.User.Id == userId)
                .OrderByDescending(preferences => preferences.Score)
                .Take(count)
                .Select(preferences => new MoviePreferenceDisplay
                {
                    MovieId = preferences.Movie.Id,
                    Title = preferences.Movie.Title,
                    Score = preferences.Score
                })
                .ToListAsync();
        }

        public async Task<string> GetUsernameAsync(int userId)
        {
            User? user = await _context.Users.FindAsync(userId);
            return user?.Username ?? $"{FallbackUsernamePrefix} {userId}";
        }

    }
}

