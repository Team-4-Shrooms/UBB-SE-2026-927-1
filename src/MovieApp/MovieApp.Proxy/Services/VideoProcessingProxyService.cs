using System.Threading.Tasks;
using MovieApp.Logic.Features.ReelsEditing;
using MovieApp.WebApi.DTOs;

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
            var payload = new { VideoUrl = videoPath, CropDataJson = cropDataJson };

            var result = await _apiClient.PostAsync<object, VideoProcessingResponse>("api/video-processing/crop", payload);
            System.Diagnostics.Debug.WriteLine($"result: {result}");

            return result?.OutputPath ?? string.Empty;
        }

        public async Task<string> MergeAudioAsync(string videoPath, int musicTrackId, double startOffsetSec, double musicDurationSec, double musicVolumePercent)
        {
            var payload = new
            {
                VideoPath = videoPath,
                MusicTrackId = musicTrackId,
                StartOffsetSec = startOffsetSec,
                MusicDurationSec = musicDurationSec,
                MusicVolumePercent = musicVolumePercent
            };
            var result = await _apiClient.PostAsync<object, VideoProcessingResponse>("api/video-processing/merge-audio", payload);
            System.Diagnostics.Debug.WriteLine($"result: {result}");
            return result?.OutputPath ?? string.Empty;
        }
    }
}
