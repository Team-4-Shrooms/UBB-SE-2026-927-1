namespace MovieApp.Features.TrailerScraping.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;
    using Google.Apis.YouTube.v3.Data;

    /// <summary>
    /// Searches YouTube using the Data API v3 to find trailer videos.
    /// Owner: Andrei.
    /// </summary>
    public class YouTubeScraperService : IYouTubeScraperService
    {
        private const int DefaultMaxResults = 5;
        private const string YouTubeAppName = "MeioAI-TrailerScraper";
        private const string SearchPartSnippet = "snippet";
        private const string SearchTypeVideo = "video";
        private const string FilmAndAnimationCategoryId = "1";

        private readonly string apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="YouTubeScraperService"/> class.
        /// </summary>
        /// <param name="apiKey">The YouTube Data API key.</param>
        public YouTubeScraperService(string apiKey)
        {
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Scrapes trailer URLs for a given movie title.
        /// </summary>
        /// <param name="movieTitle">The title of the movie to search for.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of trailer URLs.</returns>
        public async Task<IList<string>> ScrapeTrailerUrlsAsync(string movieTitle)
        {
            IList<ScrapedVideoResult> results = await this.SearchVideosAsync(movieTitle, DefaultMaxResults);
            return results.Select(result => result.VideoUrl).ToList();
        }

        /// <summary>
        /// Performs a YouTube Search.List call and returns rich results.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="maxResults">The maximum number of results to fetch.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of scraped video results.</returns>
        public async Task<IList<ScrapedVideoResult>> SearchVideosAsync(string query, int maxResults)
        {
            YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = this.apiKey,
                ApplicationName = YouTubeAppName
            });

            SearchResource.ListRequest searchRequest = youtubeService.Search.List(SearchPartSnippet);
            searchRequest.Q = query;
            searchRequest.MaxResults = maxResults;
            searchRequest.Type = SearchTypeVideo;
            searchRequest.VideoCategoryId = FilmAndAnimationCategoryId;
            searchRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            searchRequest.SafeSearch = SearchResource.ListRequest.SafeSearchEnum.Moderate;

            SearchListResponse searchResponse = await searchRequest.ExecuteAsync();

            List<ScrapedVideoResult> results = new List<ScrapedVideoResult>();
            if (searchResponse?.Items is null)
            {
                return results;
            }

            foreach (SearchResult item in searchResponse.Items)
            {
                if (item.Id?.VideoId is null)
                {
                    continue;
                }

                results.Add(new ScrapedVideoResult
                {
                    VideoId = item.Id.VideoId,
                    Title = item.Snippet?.Title ?? string.Empty,
                    ThumbnailUrl = item.Snippet?.Thumbnails?.High?.Url
                                    ?? item.Snippet?.Thumbnails?.Medium?.Url
                                    ?? item.Snippet?.Thumbnails?.Default__?.Url
                                    ?? string.Empty,
                    ChannelTitle = item.Snippet?.ChannelTitle ?? string.Empty,
                    Description = item.Snippet?.Description ?? string.Empty,
                });
            }

            return results;
        }
    }
}
