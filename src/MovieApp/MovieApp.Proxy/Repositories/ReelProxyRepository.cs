using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class ReelProxyRepository : IReelRepository
    {
        private readonly ApiClient _apiClient;

        public ReelProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<IList<Reel>> GetUserReelsAsync(int userId)
        {
            var result = await _apiClient.GetAsync<List<Reel>>($"api/reels/users/{userId}");
            return result ?? new List<Reel>();
        }

        public async Task<Reel?> GetReelByIdAsync(int reelId)
        {
            return await _apiClient.GetAsync<Reel>($"api/reels/{reelId}");
        }

        public async Task<int> UpdateReelEditsAsync(int reelId, string cropDataJson, int? backgroundMusicId, string videoUrl)
        {
            var result = await _apiClient.PutAsync<object, int>($"api/reels/{reelId}", new
            {
                CropDataJson = cropDataJson,
                BackgroundMusicId = backgroundMusicId,
                VideoUrl = videoUrl,
            });
            return result;
        }

        public async Task DeleteReelAsync(int reelId)
        {
            await _apiClient.DeleteAsync($"api/reels/{reelId}");
        }
    }
}
