using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.ReelsFeed
{
    /// <summary>
    /// Provides recommended reels for the user's feed.
    /// Owner: Tudor.
    /// </summary>
    public interface IRecommendationService
    {
        /// <summary>
        /// Gets a ranked list of recommended reels for a user.
        /// </summary>
        /// <param name="userId">The ID of the user receiving recommendations.</param>
        /// <param name="count">The maximum number of reels to return.</param>
        /// <returns>A list of recommended reels ordered from most to least relevant.</returns>
        Task<IList<Reel>> GetRecommendedReelsAsync(int userId, int count);

        Task<bool> UserHasPreferencesAsync(int userId);

        Task<IList<Reel>> GetAllReelsAsync();

        Task<Dictionary<int, decimal>> GetUserPreferenceScoresAsync(int userId);

        Task<Dictionary<int, int>> GetAllLikeCountsAsync();

        Task<List<UserReelInteraction>> GetLikesWithinDaysAsync(int days);

    }
}
