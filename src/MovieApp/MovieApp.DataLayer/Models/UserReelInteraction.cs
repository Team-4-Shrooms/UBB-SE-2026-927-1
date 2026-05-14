namespace MovieApp.DataLayer.Models
{
    public class UserReelInteraction
    {
        public long Id { get; set; }
        public bool IsLiked { get; set; }
        public decimal WatchDurationSeconds { get; set; }
        public decimal WatchPercentage { get; set; }
        public DateTime ViewedAt { get; set; }
        public int UserId { get; set; }
        public int ReelId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual User User { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Reel Reel { get; set; }
    }
}
