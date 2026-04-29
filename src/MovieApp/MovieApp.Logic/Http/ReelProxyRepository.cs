using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Http
{
    public class ReelProxyRepository : IReelRepository
    {
        private readonly ApiClient _apiClient;

        public ReelProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IList<Reel>> GetUserReelsAsync(int userId)
        {
            return await _apiClient.GetAllAsync<Reel>($"api/reels/users/{userId}");
        }

        public async Task<Reel?> GetReelByIdAsync(int reelId)
        {
            return await _apiClient.GetAsync<Reel>($"api/reels/{reelId}");
        }

        public async Task<int> UpdateReelEditsAsync(int reelId, string cropDataJson, int? backgroundMusicId, string videoUrl)
        {
            var payload = new 
            { 
                cropDataJson, 
                backgroundMusicId, 
                videoUrl 
            };
            
            await _apiClient.PutAsync($"api/reels/{reelId}", payload);
            return 1; 
        }

        public async Task DeleteReelAsync(int reelId)
        {
            await _apiClient.DeleteAsync($"api/reels/{reelId}");
        }
    }
}