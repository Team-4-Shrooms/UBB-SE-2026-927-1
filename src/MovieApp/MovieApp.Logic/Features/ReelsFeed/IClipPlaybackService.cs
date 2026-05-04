namespace MovieApp.Logic.Features.ReelsFeed
{
    /// <summary>
    /// Provides clip source prefetching for the auto-playing feed.
    /// Owner: Tudor.
    /// </summary>
    public interface IClipPlaybackService
    {
        /// <summary>
        /// Prefetches a clip URL so playback creation can be optimized by the caller.
        /// Existing cached URLs are skipped, and invalid URLs are ignored silently.
        /// </summary>
        /// <param name="videoUrl">The clip URL to prefetch.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task PrefetchClipAsync(string videoUrl);

        /// <summary>
        /// Returns transmission data needed by the presentation layer to build playback objects.
        /// </summary>
        /// <param name="videoUrl">The clip URL to resolve.</param>
        /// <returns>A DTO with playback transmission data.</returns>
        ClipMediaSourceTransmission GetClipTransmission(string videoUrl);
    }
}
