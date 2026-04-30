namespace MovieApp.Logic.Features.TrailerScraping
{
    /// <summary>
    /// Result returned by the YouTube scraper for each video found.
    /// </summary>
    public class ScrapedVideoResult
    {
        private const string YouTubeBaseUrl = "https://www.youtube.com/watch?v=";

        public string VideoId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string ChannelTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VideoUrl => $"{YouTubeBaseUrl}{this.VideoId}";
    }
}
