namespace MovieApp.Logic.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int TotalLikes { get; set; }
        public long TotalWatchTimeSeconds { get; set; }
        public decimal AverageWatchTimeSeconds { get; set; }
        public int TotalClipsViewed { get; set; }
        public decimal LikeToViewRatio { get; set; }
        public DateTime LastUpdated { get; set; }

        public User User { get; set; }
    }
}
