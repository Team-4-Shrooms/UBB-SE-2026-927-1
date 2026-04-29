using MovieApp.Logic.Models;

namespace MovieApp.Features.ReelsFeed.Services
{
    /// <summary>
    /// Manages the user's engagement profile based on aggregated interaction metrics.
    /// Current implementation supports reading the cached profile and refreshing it
    /// by recomputing watch and like statistics from reel interactions.
    /// Owner: Tudor.
    /// </summary>
    public interface IEngagementProfileService
    {
        /// <summary>
        /// Retrieves the persisted engagement profile for a user.
        /// In the current implementation, this delegates directly to the profile repository.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user's engagement profile, or null if no profile exists.</returns>
        Task<UserProfile?> GetProfileAsync(int userId);

        /// <summary>
        /// Recomputes engagement metrics from user interactions and persists the updated profile.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RefreshProfileAsync(int userId);
    }
}
