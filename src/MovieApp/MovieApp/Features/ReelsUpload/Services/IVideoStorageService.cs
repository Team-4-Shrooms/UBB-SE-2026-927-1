using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Features.ReelsUpload.Models;

namespace MovieApp.Features.ReelsUpload.Services
{
    /// <summary>
    /// Handles uploading and validating video files for Reels.
    /// Owner: Alex
    /// </summary>
    public interface IVideoStorageService
    {
        /// <summary>
        /// Uploads a video file from the local disk, inserts it into the database, and returns the stored Reel.
        /// </summary>
        Task<Reel> UploadVideoAsync(ReelUploadRequest request);

        /// <summary>
        /// Validates that a video file meets size, duration, and format requirements.
        /// </summary>
        Task<bool> ValidateVideoAsync(string localFilePath);
    }
}
