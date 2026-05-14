namespace MovieApp.Web.ViewModels.ReelsEditing
{
    using System.Collections.Generic;

    public sealed class ReelsEditingEditorViewModel
    {
        public int ReelId { get; set; }
        public string ReelTitle { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;

        public List<MusicTrackViewModel> AvailableMusicTracks { get; set; } = new List<MusicTrackViewModel>();
        public string? CurrentMusicTrackName { get; set; }
        public int? SelectedMusicTrackId { get; set; }

        public double CropMarginLeft { get; set; }
        public double CropMarginTop { get; set; }
        public double CropMarginRight { get; set; }
        public double CropMarginBottom { get; set; }

        public double MusicStartTime { get; set; }
        public double MusicDuration { get; set; } = 30.0;
        public double MusicVolume { get; set; } = 80.0;

        public string? StatusMessage { get; set; }
        public bool IsStatusSuccess { get; set; } = true;
    }
}
