namespace MovieApp.Features.TrailerScraping.Services
{
    /// <summary>
    /// Result returned by the YouTube scraper for each video found.
    /// </summary>
    public class ScrapedVideoResult
    {
        private const string YouTubeBaseUrl = "https://www.youtube.com/watch?v=";

        /// <summary>
        /// Gets or sets the unique identifier of the video.
        /// </summary>
        public string VideoId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title of the video.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the video thumbnail.
        /// </summary>
        public string ThumbnailUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title of the channel that uploaded the video.
        /// </summary>
        public string ChannelTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the video.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets the full URL of the YouTube video.
        /// </summary>
        public string VideoUrl => $"{YouTubeBaseUrl}{this.VideoId}";
    }
}