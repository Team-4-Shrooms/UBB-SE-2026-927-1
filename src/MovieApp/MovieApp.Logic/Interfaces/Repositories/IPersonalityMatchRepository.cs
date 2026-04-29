using MovieApp.Logic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.Logic.Interfaces.Repositories
{
    /// <summary>
    /// Defines the data access contract for the personality match feature, providing methods
    /// to retrieve user preferences, profiles, and movie information required for computing
    /// and displaying personality match results.
    /// </summary>
    public interface IPersonalityMatchRepository
    {
        Task<List<UserMoviePreference>> GetAllPreferencesExceptUserAsync(int excludedUserId);
        Task<List<UserMoviePreference>> GetCurrentUserPreferencesAsync(int userId);
        Task<UserProfile?> GetUserProfileAsync(int userId);
        Task<List<int>> GetRandomUserIdsAsync(int excludedUserId, int userIdsCount);

        /// <summary>
        /// Retrieves the top preferences for a user, mapped to display models containing the movie titles.
        /// </summary>
        Task<List<MoviePreferenceDisplay>> GetTopPreferencesWithTitlesAsync(int userId, int count);

        /// <summary>
        /// Retrieves the username for a specific user ID.
        /// </summary>
        Task<string> GetUsernameAsync(int userId);
    }
}
