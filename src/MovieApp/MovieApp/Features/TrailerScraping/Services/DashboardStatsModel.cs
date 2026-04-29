namespace MovieApp.Features.TrailerScraping.Services
{
    /// <summary>
    /// Dashboard statistics returned by the repository.
    /// </summary>
    public class DashboardStatsModel
    {
        /// <summary>
        /// Gets or sets the total number of movies in the database.
        /// </summary>
        public int TotalMovies { get; set; }

        /// <summary>
        /// Gets or sets the total number of reels in the database.
        /// </summary>
        public int TotalReels { get; set; }

        /// <summary>
        /// Gets or sets the total number of scrape jobs.
        /// </summary>
        public int TotalJobs { get; set; }

        /// <summary>
        /// Gets or sets the number of scrape jobs that are currently running.
        /// </summary>
        public int RunningJobs { get; set; }

        /// <summary>
        /// Gets or sets the number of scrape jobs that have successfully completed.
        /// </summary>
        public int CompletedJobs { get; set; }

        /// <summary>
        /// Gets or sets the number of scrape jobs that have failed.
        /// </summary>
        public int FailedJobs { get; set; }
    }
}