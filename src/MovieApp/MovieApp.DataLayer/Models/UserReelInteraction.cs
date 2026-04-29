namespace MovieApp.DataLayer.Models
{
    public class UserReelInteraction
    {
        public long Id { get; set; }
        public bool IsLiked { get; set; }
        public decimal WatchDurationSeconds { get; set; }
        public decimal WatchPercentage { get; set; }
        public DateTime ViewedAt { get; set; }

        public User User { get; set; }
        public Reel Reel { get; set; }
    }
}
