using MovieApp.DataLayer.Models;

namespace MovieApp.Features.ReelsFeed.Services
{
    /// <summary>
    /// Provides recommended reels for the user's feed.
    /// Current implementation uses personalized ranking when preference data exists,
    /// and falls back to popularity-based cold-start recommendations otherwise.
    /// Owner: Tudor.
    /// </summary>
    public interface IRecommendationService
    {
        /// <summary>
        /// Gets a ranked list of recommended reels for a user.
        /// For users with preferences, reels are prioritized by movie-affinity score.
        /// For users without preferences, reels are prioritized by recent global likes.
        /// </summary>
        /// <param name="userId">The ID of the user receiving recommendations.</param>
        /// <param name="count">The maximum number of reels to return.</param>
        /// <returns>A list of recommended reels ordered from most to least relevant.</returns>
        Task<IList<Reel>> GetRecommendedReelsAsync(int userId, int count);
    }
}
