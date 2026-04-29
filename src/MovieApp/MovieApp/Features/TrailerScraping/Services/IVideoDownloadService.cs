namespace MovieApp.Features.TrailerScraping.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the contract for downloading videos from external sources.
    /// </summary>
    public interface IVideoDownloadService
    {
        /// <summary>
        /// Gets the last error message encountered during a download operation, if any.
        /// </summary>
        string? LastError { get; }

        /// <summary>
        /// Ensures that all required external dependencies (like yt-dlp or ffmpeg) are available.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task EnsureDependenciesAsync();

        /// <summary>
        /// Downloads a video as an MP4 file from the specified URL.
        /// </summary>
        /// <param name="youtubeUrl">The URL of the video to download.</param>
        /// <param name="maxDurationSeconds">The maximum allowed duration for the downloaded video, in seconds.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the local path to the downloaded MP4 file, or null if the download failed.</returns>
        Task<string?> DownloadVideoAsMp4Async(string youtubeUrl, int maxDurationSeconds = 60);

        /// <summary>
        /// Gets the expected local file path for a downloaded video based on its unique identifier.
        /// </summary>
        /// <param name="videoId">The unique identifier of the video.</param>
        /// <returns>The expected local file path.</returns>
        string GetExpectedFilePath(string videoId);
    }
}