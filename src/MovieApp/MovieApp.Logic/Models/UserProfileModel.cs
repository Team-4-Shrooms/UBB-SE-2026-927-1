using System;

namespace MovieApp.Logic.Models
{
    public class UserProfileModel
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int TotalLikes { get; set; }
        public long TotalWatchTimeSec { get; set; }
        public double AvgWatchTimeSec { get; set; }
        public int TotalClipsViewed { get; set; }
        public double LikeToViewRatio { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
