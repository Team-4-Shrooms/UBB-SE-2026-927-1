using System;

namespace MovieApp.Logic.Models
{
    public class UserReelInteractionModel
    {
        public long Id { get; set; }
        public User User { get; set; }
        public ReelModel Reel { get; set; }
        public bool IsLiked { get; set; }
        public double WatchDurationSec { get; set; }
        public double WatchPercentage { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
