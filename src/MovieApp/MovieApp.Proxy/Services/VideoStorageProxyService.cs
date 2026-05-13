using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.ReelsUpload;

namespace MovieApp.Proxy.Services
{
    public class VideoStorageProxyService : IVideoStorageService
    {
        private readonly ApiClient _apiClient;

        public VideoStorageProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Reel> UploadVideoAsync(ReelUploadRequest request)
        {
            var result = await _apiClient.PostAsync<ReelUploadRequest, Reel>("api/video-storage/reels", request);
            return result ?? new Reel();
        }

        public async Task<bool> ValidateVideoAsync(string localFilePath)
        {
            return await _apiClient.GetAsync<bool>($"api/reels/validate?path={localFilePath}");
        }

        public async Task<IList<Reel>> GetUserReelsAsync(int userId)
        {
            var result = await _apiClient.GetAsync<List<Reel>>($"api/reels/user/{userId}");
            return result ?? new List<Reel>();
        }
    }
}
