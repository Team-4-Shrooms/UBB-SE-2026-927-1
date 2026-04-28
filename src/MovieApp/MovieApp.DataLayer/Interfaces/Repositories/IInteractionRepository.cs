using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Repository for managing user interactions with reels, including views, likes, and metadata.
    /// </summary>
    public interface IInteractionRepository
    {
        /// <summary>
        /// Inserts a new user-reel interaction record.
        /// </summary>
        /// <param name="interaction">The interaction model to insert.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InsertInteractionAsync(UserReelInteraction interaction);

        /// <summary>
        /// Inserts or updates a user-reel interaction.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="reelId">The ID of the reel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpsertInteractionAsync(int userId, int reelId);

        /// <summary>
        /// Toggles the like status for a user's interaction with a reel.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="reelId">The ID of the reel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ToggleLikeAsync(int userId, int reelId);

        /// <summary>
        /// Updates viewing data for a user's reel interaction.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="reelId">The ID of the reel.</param>
        /// <param name="watchDurationSeconds">The duration watched in seconds.</param>
        /// <param name="watchPercentage">The percentage of the reel watched (0-100).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateViewDataAsync(int userId, int reelId, decimal watchDurationSeconds, decimal watchPercentage);

        /// <summary>
        /// Retrieves the interaction between a user and a reel.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="reelId">The ID of the reel.</param>
        /// <returns>The user-reel interaction, or null if no interaction exists.</returns>
        Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId);

        /// <summary>
        /// Gets the total number of likes for a reel.
        /// </summary>
        /// <param name="reelId">The ID of the reel.</param>
        /// <returns>The count of likes.</returns>
        Task<int> GetLikeCountAsync(int reelId);

        /// <summary>
        /// Retrieves the movie ID associated with a reel, if any.
        /// </summary>
        /// <param name="reelId">The ID of the reel.</param>
        /// <returns>The associated movie ID, or null if no movie is linked.</returns>
        Task<int?> GetReelMovieIdAsync(int reelId);
    }
}
