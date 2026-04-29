namespace MovieApp.Features.ReelsEditing.Models
{
    using System.Text.Json;

    /// <summary>
    /// Represents the metadata required for editing a video, including cropping and audio parameters.
    /// </summary>
    public class VideoEditMetadata
    {
        private const int DefaultCropWidth = 1920;
        private const int DefaultCropHeight = 1080;
        private const double DefaultMusicDurationSeconds = 30.0;
        private const double DefaultMusicVolumePercentage = 80.0;

        /// <summary>
        /// Gets or sets the X coordinate for the video crop area.
        /// </summary>
        public int CropXCoordinate { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate for the video crop area.
        /// </summary>
        public int CropYCoordinate { get; set; }

        /// <summary>
        /// Gets or sets the width of the cropped video.
        /// </summary>
        public int CropWidth { get; set; } = DefaultCropWidth;

        /// <summary>
        /// Gets or sets the height of the cropped video.
        /// </summary>
        public int CropHeight { get; set; } = DefaultCropHeight;

        /// <summary>
        /// Gets or sets the unique identifier of the selected background music track.
        /// </summary>
        public int? SelectedMusicTrackId { get; set; }

        /// <summary>
        /// Gets or sets the start time offset for the background music, in seconds.
        /// </summary>
        public double MusicStartTime { get; set; }

        /// <summary>
        /// Gets or sets the duration of the background music to play, in seconds.
        /// </summary>
        public double MusicDuration { get; set; } = DefaultMusicDurationSeconds;

        /// <summary>
        /// Gets or sets the volume level of the background music as a percentage.
        /// </summary>
        public double MusicVolume { get; set; } = DefaultMusicVolumePercentage;

        /// <summary>
        /// Serializes the current video edit metadata into a JSON string.
        /// </summary>
        /// <returns>A JSON formatted string containing crop and music data.</returns>
        public string ToCropDataJson()
        {
            return JsonSerializer.Serialize(new
            {
                // Keeping the JSON keys as x and y to prevent breaking the database parser
                x = this.CropXCoordinate,
                y = this.CropYCoordinate,
                width = this.CropWidth,
                height = this.CropHeight,
                musicStartTime = this.MusicStartTime,
                musicDuration = this.MusicDuration,
                musicVolume = this.MusicVolume,
            });
        }
    }
}