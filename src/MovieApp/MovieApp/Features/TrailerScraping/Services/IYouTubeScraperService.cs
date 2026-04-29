namespace MovieApp.Features.TrailerScraping.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the contract for searching and scraping videos from YouTube.
    /// </summary>
    public interface IYouTubeScraperService
    {
        /// <summary>
        /// Searches YouTube for videos matching the specified query.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="maxResults">The maximum number of results to return. Defaults to 5.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of scraped video results.</returns>
        Task<IList<ScrapedVideoResult>> SearchVideosAsync(string query, int maxResults = 5);
    }
}