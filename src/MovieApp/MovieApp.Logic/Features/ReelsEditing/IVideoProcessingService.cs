using System.Threading.Tasks;

namespace MovieApp.Logic.Features.ReelsEditing
{
    /// <summary>
    /// Defines the contract for processing and editing video files.
    /// </summary>
    public interface IVideoProcessingService
    {
        Task<string> ApplyCropAsync(string videoPath, string cropDataJson);
        Task<string> MergeAudioAsync(string videoPath, int musicTrackId, double startOffsetSec, double musicDurationSec, double musicVolumePercent);
    }
}
