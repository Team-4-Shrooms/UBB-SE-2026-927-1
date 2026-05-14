using System.Threading.Tasks;
using System.Collections.Generic;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.ReelsUpload
{
    /// <summary>
    /// Handles uploading and validating video files for Reels.
    /// </summary>
    public interface IVideoStorageService
    {
        Task<Reel> UploadVideoAsync(ReelUploadRequest request);
        Task<bool> ValidateVideoAsync(string localFilePath);
        Task<IList<Reel>> GetUserReelsAsync(int userId);
        Task<Reel> InsertReelAsync(Reel reel);

        /// <summary>
        /// Moves a locally-processed video file into permanent storage and returns
        /// the relative URL that the static-files middleware will serve.
        /// </summary>
        Task<string> StoreProcessedFileAsync(string localProcessedFilePath);
    }
}
