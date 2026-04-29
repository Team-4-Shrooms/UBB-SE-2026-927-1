using MovieApp.DataLayer.Models;
using MovieApp.Features.PersonalityMatch.Models;
namespace MovieApp.Features.PersonalityMatch.Services
{
    /// <summary>
    /// Defines the contract for the personality matching service, providing methods to retrieve compatibility-based and random user matches for a given user.
    /// </summary>
    public interface IPersonalityMatchingService
    {
        /// <summary>
        /// Retrieves the top personality matches for the specified user, ranked by compatibility score in descending order.
        /// </summary>
        /// <param name="userId">The identifier of the user for whom matches are to be computed.</param>
        /// <param name="count">The maximum number of top matches to retrieve.</param>
        /// <returns>
        /// A list of <see cref="MatchResult"/> records representing the best-matching users, ordered by match score descending, limited to the specified count.
        /// </returns>
        Task<List<MatchResult>> GetTopMatchesAsync(int userId, int count);

        /// <summary>
        /// Retrieves a random selection of users for the specified user, regardless of compatibility score.
        /// </summary>
        /// <param name="userId">The identifier of the user for whom random matches are to be retrieved.</param>
        /// <param name="count">The number of random users to retrieve.</param>
        /// <returns>
        /// A list of <see cref="MatchResult"/> records representing randomly selected users, excluding the specified user.
        /// </returns>
        Task<List<MatchResult>> GetRandomUsersAsync(int userId, int count);

        /// <summary>
        /// Retrieves the full profile of the specified user.
        /// Returns <see langword="null"/> if no profile exists for the user.
        /// </summary>
        /// <param name="userId">The identifier of the user whose profile is to be retrieved.</param>
        /// <returns>
        /// A <see cref="UserProfile"/> if found; otherwise <see langword="null"/>.
        /// </returns>
        Task<UserProfile?> GetUserProfileAsync(int userId);

        /// <summary>
        /// Retrieves the top-rated movie preferences for the specified user, enriched with movie titles.
        /// </summary>
        /// <param name="userId">The identifier of the user whose top preferences are to be retrieved.</param>
        /// <param name="topMoviePreferencesCount">The maximum number of top preferences to retrieve.</param>
        /// <returns>
        /// A list of <see cref="MoviePreferenceDisplay"/> records ordered by score descending.
        /// </returns>
        Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount);

        /// <summary>
        /// Retrieves the username of the specified user.
        /// </summary>
        /// <param name="userId">The identifier of the user whose username is to be retrieved.</param>
        /// <returns>
        /// A string containing the username of the specified user.
        /// </returns>
        Task<string> GetUsernameAsync(int userId);
    }
}
