using MovieApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Defines the contract for interacting with the reel repository.
    /// </summary>
    public interface IReelRepository
    {
        /// <summary>
        /// Retrieves all reels associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A list of reels created by the user, ordered by creation date descending.</returns>
        Task<IList<Reel>> GetUserReelsAsync(int userId);

        /// <summary>
        /// Retrieves a specific reel by its unique identifier.
        /// </summary>
        /// <param name="reelId">The unique identifier of the reel.</param>
        /// <returns>The reel if found, otherwise null.</returns>
        Task<Reel?> GetReelByIdAsync(int reelId);

        /// <summary>
        /// Updates the editing metadata and video URL for a specific reel.
        /// </summary>
        /// <param name="reelId">The unique identifier of the reel to update.</param>
        /// <param name="cropDataJson">The JSON string containing the crop metadata.</param>
        /// <param name="backgroundMusicId">The unique identifier of the background music track, if any.</param>
        /// <param name="videoUrl">The updated URL or path to the video file.</param>
        /// <returns>The number of affected rows.</returns>
        Task<int> UpdateReelEditsAsync(int reelId, string cropDataJson, int? backgroundMusicId, string videoUrl);

        /// <summary>
        /// Deletes a specific reel and its associated interactions.
        /// </summary>
        /// <param name="reelId">The unique identifier of the reel to delete.</param>
        Task DeleteReelAsync(int reelId);
    }
}
