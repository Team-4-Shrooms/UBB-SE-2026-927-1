using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    /// <summary>
    /// Repository for recommendation-related reel and preference data access.
    /// </summary>
    public interface IRecommendationRepository
    {
        /// <summary>
        /// Returns true if the user has at least one stored movie preference.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check for preferences. Must correspond to an existing user.</param>
        /// <returns><see langword="true"/> if the user has at least one stored movie preference; otherwise, <see langword="false"/>.</returns>
        Task<bool> UserHasPreferencesAsync(int userId);

        /// <summary>
        /// Retrieves all reels with associated movie metadata needed for recommendation ranking.
        /// </summary>
        /// <returns>A list of <see cref="Reel"/> objects representing all reels along with their associated movie metadata. 
        /// The list will be empty if there are no reels available.</returns>
        Task<IList<Reel>> GetAllReelsAsync();

        /// <summary>
        /// Retrieves the user's preference score per movie.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom to retrieve preference scores. Must correspond to an existing user.</param>
        /// <returns>A dictionary where the key is the movie ID and the value is the user's preference score for that movie.</returns>
        Task<Dictionary<int, decimal>> GetUserPreferenceScoresAsync(int userId);

        /// <summary>
        /// Retrieves all like counts grouped by reel (no date filtering).
        /// </summary>
        Task<Dictionary<int, int>> GetAllLikeCountsAsync();

        /// <summary>
        /// Retrieves all interactions where IsLiked = 1 and viewed within the last N days.
        /// </summary>
        /// <param name="days">Number of days to look back.</param>
        /// <returns>List of user-reel interactions that are likes within the specified time frame.</returns>
        Task<List<UserReelInteraction>> GetLikesWithinDaysAsync(int days);
    }
}
