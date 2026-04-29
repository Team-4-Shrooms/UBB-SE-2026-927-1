namespace MovieApp.DataLayer.Models
{
    /// <summary>
    /// Represents aggregated dashboard statistics for the trailer scraping admin view,
    /// including counts of movies, reels, and scrape jobs by status.
    /// </summary>
    public class DashboardStatsModel
    {
        /// <summary>Gets or sets the total number of movies in the database.</summary>
        public int TotalMovies { get; set; }

        /// <summary>Gets or sets the total number of reels in the database.</summary>
        public int TotalReels { get; set; }

        /// <summary>Gets or sets the total number of scrape jobs ever created.</summary>
        public int TotalJobs { get; set; }

        /// <summary>Gets or sets the number of scrape jobs currently running.</summary>
        public int RunningJobs { get; set; }

        /// <summary>Gets or sets the number of scrape jobs that completed successfully.</summary>
        public int CompletedJobs { get; set; }

        /// <summary>Gets or sets the number of scrape jobs that failed.</summary>
        public int FailedJobs { get; set; }
    }
}
