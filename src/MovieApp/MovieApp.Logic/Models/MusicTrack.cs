using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Logic.Models
{
    public class MusicTrack
    {
        public int Id { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
        public decimal DurationSeconds { get; set; }

        [NotMapped]
        public string FormattedDuration
        {
            get
            {
                TimeSpan ts = TimeSpan.FromSeconds((Double)DurationSeconds);
                return ts.TotalMinutes >= 1
                    ? $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}"
                    : $"0:{ts.Seconds:D2}";
            }
        }
    }
}
