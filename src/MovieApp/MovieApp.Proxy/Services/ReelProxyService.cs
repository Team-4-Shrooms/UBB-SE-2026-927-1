using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class ReelProxyService : IReelService
    {
        private readonly ApiClient _apiClient;

        public ReelProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            var result = await _apiClient.GetAsync<List<Reel>>("api/reels");
            return result ?? new List<Reel>();
        }

        public async Task<IList<Reel>> GetUserReelsAsync(int userId)
        {
            var result = await _apiClient.GetAsync<List<Reel>>($"api/reels/user/{userId}");
            return result ?? new List<Reel>();
        }

        public async Task<Reel?> GetReelByIdAsync(int reelId)
        {
            return await _apiClient.GetAsync<Reel>($"api/reels/{reelId}");
        }

        public async Task<int> UpdateReelEditsAsync(int reelId, string cropDataJson, int? backgroundMusicId, string videoUrl)
        {
            return await _apiClient.PutAsync<object, int>($"api/reels/{reelId}/edits", new { CropDataJson = cropDataJson, BackgroundMusicId = backgroundMusicId, VideoUrl = videoUrl });
        }

        public async Task DeleteReelAsync(int reelId)
        {
            await _apiClient.DeleteAsync($"api/reels/{reelId}");
        }
    }
}
