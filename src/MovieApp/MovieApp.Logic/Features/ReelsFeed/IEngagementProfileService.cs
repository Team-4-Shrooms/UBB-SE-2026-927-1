using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.ReelsFeed
{
    /// <summary>
    /// Manages the user's engagement profile based on aggregated interaction metrics.
    /// Owner: Tudor.
    /// </summary>
    public interface IEngagementProfileService
    {
        /// <summary>
        /// Retrieves the persisted engagement profile for a user.
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
