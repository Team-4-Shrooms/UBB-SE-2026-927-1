namespace MovieApp.Web.ViewModels.ReelsEditing
{
    public sealed class SaveMusicForm
    {
        public int ReelId { get; set; }
        public int MusicTrackId { get; set; }
        public double MusicStartTime { get; set; }
        public double MusicDuration { get; set; }
        public double MusicVolume { get; set; }
    }
}
