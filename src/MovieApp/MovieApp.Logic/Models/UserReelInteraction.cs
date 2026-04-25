namespace MovieApp.Logic.Models
{
    public class UserReelInteraction
    {
        public long Id { get; set; }
        public bool IsLiked { get; set; }
        public double WatchDurationSec { get; set; }
        public double WatchPercentage { get; set; }
        public DateTime ViewedAt { get; set; }

        public User User { get; set; }
        public Reel Reel { get; set; }
    }
}
