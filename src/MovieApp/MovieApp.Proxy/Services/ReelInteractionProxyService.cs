using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.ReelsFeed;

namespace MovieApp.Proxy.Services
{
    public class ReelInteractionProxyService : IReelInteractionService
    {
        private readonly ApiClient _apiClient;

        public ReelInteractionProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task ToggleLikeAsync(int userId, int reelId)
        {
            await _apiClient.PostAsync($"api/reels/{reelId}/like?userId={userId}", new { });
        }

        public async Task RecordViewAsync(int userId, int reelId, double watchDurationSec, double watchPercentage)
        {
            await _apiClient.PostAsync($"api/reels/{reelId}/view", new 
            { 
                UserId = userId, 
                WatchDurationSec = watchDurationSec, 
                WatchPercentage = watchPercentage 
            });
        }

        public async Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId)
        {
            return await _apiClient.GetAsync<UserReelInteraction>($"api/reels/{reelId}/interaction?userId={userId}");
        }

        public async Task<int> GetLikeCountAsync(int reelId)
        {
            return await _apiClient.GetAsync<int>($"api/reels/{reelId}/likes");
        }
    }
}
