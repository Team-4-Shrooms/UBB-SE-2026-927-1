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

        /// <inheritdoc />
        public async Task<Dictionary<int, List<UserMoviePreference>>> GetAllPreferencesExceptUserAsync(int excludedUserId)
        {
            List<UserMoviePreference> allPreferences = await _context.UserMoviePreferences
                .Include(preference => preference.User)
                .Include(preference => preference.Movie)
                .Where(preference => preference.User.Id != excludedUserId)
                .ToListAsync();

            Dictionary<int, List<UserMoviePreference>> preferencesByUserId = new Dictionary<int, List<UserMoviePreference>>();

            foreach (UserMoviePreference preference in allPreferences)
            {
                int userId = preference.User.Id;

                if (!preferencesByUserId.ContainsKey(userId))
                {
                    preferencesByUserId[userId] = new List<UserMoviePreference>();
                }

                preferencesByUserId[userId].Add(preference);
            }

            return preferencesByUserId;
        }

        /// <inheritdoc />
        public async Task<List<UserMoviePreference>> GetCurrentUserPreferencesAsync(int userId)
        {
            return await _context.UserMoviePreferences
                .Include(preference => preference.Movie)
                .Where(preference => preference.User.Id == userId)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<UserProfile?> GetUserProfileAsync(int userId)
        {
            return await _context.UserProfiles
                .Include(profile => profile.User)
                .FirstOrDefaultAsync(profile => profile.User.Id == userId);
        }

        /// <inheritdoc />
        public async Task<List<int>> GetRandomUserIdsAsync(int excludedUserId, int userIdsCount)
        {
            return await _context.UserMoviePreferences
                .Where(preference => preference.User.Id != excludedUserId)
                .Select(preference => preference.User.Id)
                .Distinct()
                .OrderBy(id => EF.Functions.Random())
                .Take(userIdsCount)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<string> GetUsernameAsync(int userId)
        {
            User? user = await _context.Users.FindAsync(userId);
            return user?.Username ?? $"{FallbackUsernamePrefix} {userId}";
        }

        /// <inheritdoc />
        public async Task<List<MoviePreferenceDisplay>> GetTopPreferencesWithTitlesAsync(int userId, int topMoviePreferencesCount)
        {
            List<UserMoviePreference> topPreferences = await _context.UserMoviePreferences
                .Include(preference => preference.Movie)
                .Where(preference => preference.User.Id == userId)
                .OrderByDescending(preference => preference.Score)
                .Take(topMoviePreferencesCount)
                .ToListAsync();

            List<MoviePreferenceDisplay> displayModels = new List<MoviePreferenceDisplay>();
            bool isFirst = true;

            foreach (UserMoviePreference preference in topPreferences)
            {
                displayModels.Add(new MoviePreferenceDisplay
                {
                    MovieId = preference.Movie.Id,
                    Title = preference.Movie.Title,
                    Score = preference.Score,
                    IsBestMovie = isFirst,
                });

                isFirst = false;
            }

            return displayModels;
        }
    }
}
