namespace MovieApp.Features.TrailerScraping.Services
{
    using System;
    using System.Threading.Tasks;
    using MovieApp.DataLayer.Models;

    /// <summary>
    /// Downloads and ingests scraped trailer videos into the local system.
    /// Owner: Andrei.
    /// </summary>
    public interface IVideoIngestionService
    {
        /// <summary>
        /// Ingests a video from the specified URL and links it to the given movie ID.
        /// </summary>
        /// <param name="trailerUrl">The URL of the trailer to ingest.</param>
        /// <param name="movieId">The unique identifier of the associated movie.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ingested video identifier or path as a string.</returns>
        Task<string> IngestVideoFromUrlAsync(string trailerUrl, int movieId);

        /// <summary>
        /// Runs a comprehensive scrape job for a specific movie.
        /// </summary>
        /// <param name="movie">The movie model containing the details for the scrape job.</param>
        /// <param name="maxResults">The maximum number of results to fetch.</param>
        /// <param name="onLogEntry">An optional callback function to handle log entries generated during the scrape job.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the completed scrape job model.</returns>
        Task<ScrapeJob> RunScrapeJobAsync(
            Movie movie,
            int maxResults,
            Func<ScrapeJobLog, Task>? onLogEntry = null);
    }
}