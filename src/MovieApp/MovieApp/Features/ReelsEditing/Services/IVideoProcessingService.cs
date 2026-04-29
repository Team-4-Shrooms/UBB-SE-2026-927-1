namespace MovieApp.Features.ReelsEditing.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the contract for processing and editing video files.
    /// </summary>
    public interface IVideoProcessingService
    {
        /// <summary>
        /// Applies a crop to the specified video based on the provided JSON metadata.
        /// </summary>
        /// <param name="videoPath">The path or URL to the source video file.</param>
        /// <param name="cropDataJson">The JSON string containing the crop dimensions and coordinates.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the path to the newly cropped video file.</returns>
        Task<string> ApplyCropAsync(string videoPath, string cropDataJson);

        /// <summary>
        /// Merges an audio track into the specified video file.
        /// </summary>
        /// <param name="videoPath">The path or URL to the source video file.</param>
        /// <param name="musicTrackId">The unique identifier of the background music track.</param>
        /// <param name="startOffsetSec">The start time offset for the audio track, in seconds.</param>
        /// <param name="musicDurationSec">The duration of the audio to play, in seconds.</param>
        /// <param name="musicVolumePercent">The volume level of the music as a percentage.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the path to the video file with the merged audio.</returns>
        Task<string> MergeAudioAsync(
            string videoPath,
            int musicTrackId,
            double startOffsetSec,
            double musicDurationSec,
            double musicVolumePercent);
    }
}