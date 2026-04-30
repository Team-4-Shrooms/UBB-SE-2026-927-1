using System;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.TrailerScraping
{
    /// <summary>
    /// Downloads and ingests scraped trailer videos into the local system.
    /// Owner: Andrei.
    /// </summary>
    public interface IVideoIngestionService
    {
        Task<string> IngestVideoFromUrlAsync(string trailerUrl, int movieId);
        Task<ScrapeJob> RunScrapeJobAsync(Movie movie, int maxResults, Func<ScrapeJobLog, Task>? onLogEntry = null);
    }
}
