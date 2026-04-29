using MovieApp.DataLayer.Models;

namespace MovieApp.Features.ReelsFeed.Services
{
    /// <summary>
    /// Orchestrates user interactions (likes and views) with reels.
    /// Current implementation delegates persistence to the interaction repository
    /// and boosts movie preference only when a reel transitions to liked.
    /// Owner: Tudor.
    /// </summary>
    public interface IReelInteractionService
    {
        /// <summary>
        /// Toggles the like state for a user-reel interaction.
        /// If the reel changes from unliked to liked, the associated movie preference is boosted.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="reelId">The ID of the reel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ToggleLikeAsync(int userId, int reelId);

        /// <summary>
        /// Records or updates view metrics for a user-reel interaction.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="reelId">The ID of the reel.</param>
        /// <param name="watchDurationSec">The watched duration in seconds.</param>
        /// <param name="watchPercentage">The watched percentage for the reel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RecordViewAsync(int userId, int reelId, double watchDurationSec, double watchPercentage);

        /// <summary>
        /// Retrieves the interaction state for a user and reel.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="reelId">The ID of the reel.</param>
        /// <returns>The interaction model, or null if no interaction exists.</returns>
        Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId);

        /// <summary>
        /// Gets the total number of likes recorded for a reel.
        /// </summary>
        /// <param name="reelId">The ID of the reel.</param>
        /// <returns>The total like count for the reel.</returns>
        Task<int> GetLikeCountAsync(int reelId);
    }
}
