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
        /// <summary>
        /// Retrieves all movie preferences for every user except the specified one,
        /// grouped by user identifier.
        /// </summary>
        /// <param name="excludedUserId">The identifier of the user whose preferences should be excluded.</param>
        /// <returns>
        /// A dictionary mapping each user's identifier to their list of movie preference records.
        /// </returns>
        Task<Dictionary<int, List<UserMoviePreference>>> GetAllPreferencesExceptUserAsync(int excludedUserId);

        /// <summary>
        /// Retrieves all movie preferences belonging to the specified user.
        /// </summary>
        /// <param name="userId">The identifier of the user whose preferences are to be retrieved.</param>
        /// <returns>A list of <see cref="UserMoviePreference"/> records for the specified user.</returns>
        Task<List<UserMoviePreference>> GetCurrentUserPreferencesAsync(int userId);

        /// <summary>
        /// Retrieves the profile of the specified user.
        /// </summary>
        /// <param name="userId">The identifier of the user whose profile is to be retrieved.</param>
        /// <returns>A <see cref="UserProfile"/> if the user exists, otherwise null.</returns>
        Task<UserProfile?> GetUserProfileAsync(int userId);

        /// <summary>
        /// Retrieves a random selection of user identifiers, excluding the specified user.
        /// </summary>
        /// <param name="excludedUserId">The identifier of the user to exclude from the selection.</param>
        /// <param name="userIdsCount">The number of random user identifiers to retrieve.</param>
        /// <returns>A list of randomly selected user identifiers.</returns>
        Task<List<int>> GetRandomUserIdsAsync(int excludedUserId, int userIdsCount);

        /// <summary>
        /// Retrieves the username of the specified user.
        /// </summary>
        /// <param name="userId">The identifier of the user whose username is to be retrieved.</param>
        /// <returns>A string containing the username, or a fallback placeholder if not found.</returns>
        Task<string> GetUsernameAsync(int userId);

        /// <summary>
        /// Retrieves the top-rated movie preferences for the specified user,
        /// enriched with movie titles, limited to the given count.
        /// </summary>
        /// <param name="userId">The identifier of the user whose top preferences are to be retrieved.</param>
        /// <param name="topMoviePreferencesCount">The maximum number of top preferences to retrieve.</param>
        /// <returns>
        /// A list of <see cref="MoviePreferenceDisplay"/> records representing the user's
        /// highest-scored movies, ordered by score descending.
        /// </returns>
        Task<List<MoviePreferenceDisplay>> GetTopPreferencesWithTitlesAsync(int userId, int topMoviePreferencesCount);
    }
}
