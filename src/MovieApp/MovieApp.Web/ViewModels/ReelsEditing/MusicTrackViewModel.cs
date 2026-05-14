namespace MovieApp.Web.ViewModels.ReelsEditing
{
    public sealed class MusicTrackViewModel
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string FormattedDuration { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
    }
}
