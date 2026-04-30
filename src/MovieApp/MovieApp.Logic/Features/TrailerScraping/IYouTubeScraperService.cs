using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.Logic.Features.TrailerScraping
{
    /// <summary>
    /// Defines the contract for searching and scraping videos from YouTube.
    /// </summary>
    public interface IYouTubeScraperService
    {
        Task<IList<ScrapedVideoResult>> SearchVideosAsync(string query, int maxResults = 5);
    }
}
