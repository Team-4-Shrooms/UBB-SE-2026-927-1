using System.Threading.Tasks;
using MovieApp.Logic.Features.ReelsEditing;

namespace MovieApp.Proxy.Services
{
    public class VideoProcessingProxyService : IVideoProcessingService
    {
        private readonly ApiClient _apiClient;

        public VideoProcessingProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<string> ApplyCropAsync(string videoPath, string cropDataJson)
        {
            var result = await _apiClient.PostAsync<object, string>("api/video/crop", new { VideoPath = videoPath, CropDataJson = cropDataJson });
            return result ?? string.Empty;
        }

        public async Task<string> MergeAudioAsync(string videoPath, int musicTrackId, double startOffsetSec, double musicDurationSec, double musicVolumePercent)
        {
            var result = await _apiClient.PostAsync<object, string>("api/video/merge", new 
            { 
                VideoPath = videoPath, 
                MusicTrackId = musicTrackId, 
                StartOffsetSec = startOffsetSec, 
                MusicDurationSec = musicDurationSec, 
                MusicVolumePercent = musicVolumePercent 
            });
            return result ?? string.Empty;
        }
    }
}
