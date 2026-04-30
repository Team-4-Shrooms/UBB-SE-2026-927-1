namespace MovieApp.Logic.Features.TrailerScraping
{
    /// <summary>
    /// Dashboard statistics returned by the repository.
    /// </summary>
    public class DashboardStatsModel
    {
        public int TotalMovies { get; set; }
        public int TotalReels { get; set; }
        public int TotalJobs { get; set; }
        public int RunningJobs { get; set; }
        public int CompletedJobs { get; set; }
        public int FailedJobs { get; set; }
    }
}
