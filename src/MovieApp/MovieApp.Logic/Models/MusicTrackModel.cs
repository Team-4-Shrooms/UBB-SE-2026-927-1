using System;

namespace MovieApp.Logic.Models
{
    public class MusicTrackModel
    {
        public int Id { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
        public double DurationSeconds { get; set; }

        public string FormattedDuration
        {
            get
            {
                var ts = TimeSpan.FromSeconds(DurationSeconds);
                return ts.TotalMinutes >= 1
                    ? $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}"
                    : $"0:{ts.Seconds:D2}";
            }
        }
    }
}
