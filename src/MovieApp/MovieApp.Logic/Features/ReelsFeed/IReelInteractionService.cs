using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.ReelsFeed
{
    /// <summary>
    /// Orchestrates user interactions (likes and views) with reels.
    /// Owner: Tudor.
    /// </summary>
    public interface IReelInteractionService
    {
        /// <summary>
        /// Toggles the like state for a user-reel interaction.
        /// If the reel changes from unliked to liked, the associated movie preference is boosted.
        /// </summary>
        Task ToggleLikeAsync(int userId, int reelId);

        /// <summary>
        /// Records or updates view metrics for a user-reel interaction.
        /// </summary>
        Task RecordViewAsync(int userId, int reelId, double watchDurationSec, double watchPercentage);

        /// <summary>
        /// Retrieves the interaction state for a user and reel.
        /// </summary>
        Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId);

        /// <summary>
        /// Gets the total number of likes recorded for a reel.
        /// </summary>
        Task<int> GetLikeCountAsync(int reelId);
    }
}
