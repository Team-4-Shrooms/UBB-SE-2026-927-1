using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.Logic.Features.TrailerScraping
{
    /// <summary>
    /// Scrapes trailer metadata and URLs from external sources.
    /// </summary>
    public interface IWebScraperService
    {
        /// <summary>
        /// Scrapes trailer URLs for a given movie title.
        /// </summary>
        Task<IList<string>> ScrapeTrailerUrlsAsync(string movieTitle);
    }
}
