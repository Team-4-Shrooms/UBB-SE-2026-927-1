namespace MovieApp.Features.TrailerScraping.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Scrapes trailer metadata and URLs from external sources.
    /// Owner: Andrei.
    /// </summary>
    public interface IWebScraperService
    {
        /// <summary>
        /// Scrapes trailer URLs for a given movie title.
        /// </summary>
        /// <param name="movieTitle">The title of the movie to search for.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of trailer URLs.</returns>
        Task<IList<string>> ScrapeTrailerUrlsAsync(string movieTitle);
    }
}