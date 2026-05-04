using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace MovieApp.Logic.Features.TrailerScraping
{
    /// <summary>
    /// Searches YouTube using the Data API v3 to find trailer videos.
    /// </summary>
    public class YouTubeScraperService : IYouTubeScraperService, IWebScraperService
    {
        private const int DefaultMaxResults = 5;
        private const string YouTubeAppName = "MeioAI-TrailerScraper";
        private const string SearchPartSnippet = "snippet";
        private const string SearchTypeVideo = "video";
        private const string FilmAndAnimationCategoryId = "1";

        private readonly string apiKey;

        public YouTubeScraperService(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<IList<string>> ScrapeTrailerUrlsAsync(string movieTitle)
        {
            IList<ScrapedVideoResult> results = await this.SearchVideosAsync(movieTitle, DefaultMaxResults);
            return results.Select(result => result.VideoUrl).ToList();
        }

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
            if (searchResponse?.Items is null) return results;

            foreach (SearchResult item in searchResponse.Items)
            {
                if (item.Id?.VideoId is null) continue;

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
