using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Repository for managing ScrapeJob and ScrapeJobLog records.
    /// </summary>
    public interface IScrapeJobRepository
    {
        /// <summary>
        /// Creates a new scrape job and returns its auto-generated ID.
        /// </summary>
        /// <param name="job">The scrape job to insert.</param>
        /// <returns>The auto-generated ID of the created job.</returns>
        Task<int> CreateJobAsync(ScrapeJob job);

        /// <summary>
        /// Updates an existing scrape job's status, counts, completion time, and error message.
        /// </summary>
        /// <param name="job">The scrape job with updated values.</param>
        Task UpdateJobAsync(ScrapeJob job);

        /// <summary>
        /// Appends a log entry to a scrape job.
        /// </summary>
        /// <param name="log">The log entry to insert.</param>
        Task AddLogEntryAsync(ScrapeJobLog log);

        /// <summary>
        /// Retrieves all scrape jobs ordered by most recent first.
        /// </summary>
        /// <returns>A list of all scrape jobs.</returns>
        Task<IList<ScrapeJob>> GetAllJobsAsync();

        /// <summary>
        /// Retrieves log entries for a specific job ordered by timestamp.
        /// </summary>
        /// <param name="jobId">The unique identifier of the scrape job.</param>
        /// <returns>A list of log entries for the specified job.</returns>
        Task<IList<ScrapeJobLog>> GetLogsForJobAsync(int jobId);

        /// <summary>
        /// Retrieves all log entries across all jobs, most recent first.
        /// </summary>
        /// <returns>A list of all log entries.</returns>
        Task<IList<ScrapeJobLog>> GetAllLogsAsync();

        /// <summary>
        /// Returns aggregated dashboard statistics.
        /// </summary>
        /// <returns>The aggregated dashboard statistics.</returns>
        Task<DashboardStatsModel> GetDashboardStatsAsync();

        /// <summary>
        /// Searches movies by partial title match for autocomplete.
        /// </summary>
        /// <param name="partialName">The partial name or title to search for.</param>
        /// <returns>A list of matching movies.</returns>
        Task<IList<Movie>> SearchMoviesByNameAsync(string partialName);

        /// <summary>
        /// Checks whether a movie with the given title already exists.
        /// </summary>
        /// <param name="title">The exact title of the movie.</param>
        /// <returns>The movie ID if found, null otherwise.</returns>
        Task<int?> FindMovieByTitleAsync(string title);

        /// <summary>
        /// Checks whether a reel with the given VideoUrl already exists.
        /// </summary>
        /// <param name="videoUrl">The video URL to check for duplicates.</param>
        /// <returns>True if the reel exists, false otherwise.</returns>
        Task<bool> ReelExistsByVideoUrlAsync(string videoUrl);

        /// <summary>
        /// Inserts a new scraped reel and returns its auto-generated ID.
        /// </summary>
        /// <param name="reel">The reel to insert.</param>
        /// <returns>The auto-generated ID of the inserted reel.</returns>
        Task<int> InsertScrapedReelAsync(Reel reel);

        /// <summary>
        /// Retrieves all movies from the database.
        /// </summary>
        /// <returns>A list of all movies.</returns>
        Task<IList<Movie>> GetAllMoviesAsync();

        /// <summary>
        /// Retrieves all reels from the database.
        /// </summary>
        /// <returns>A list of all reels.</returns>
        Task<IList<Reel>> GetAllReelsAsync();
    }
}
