using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
{
    public class InteractionProxyRepository : IInteractionRepository
    {
        private readonly ApiClient _apiClient;

        public InteractionProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task InsertInteractionAsync(UserReelInteraction interaction)
        {
            await _apiClient.PostAsync("api/interactions", interaction);
        }

        public async Task UpsertInteractionAsync(int userId, int reelId)
        {
            await _apiClient.PostAsync($"api/interactions/users/{userId}/reels/{reelId}", new { });
        }

        public async Task ToggleLikeAsync(int userId, int reelId)
        {
            await _apiClient.PutAsync($"api/interactions/users/{userId}/reels/{reelId}/like", new { });
        }

        public async Task UpdateViewDataAsync(int userId, int reelId, decimal watchDurationSeconds, decimal watchPercentage)
        {
            var payload = new { watchDurationSeconds, watchPercentage };
            await _apiClient.PutAsync($"api/interactions/users/{userId}/reels/{reelId}/view", payload);
        }

        public async Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId)
        {
            var results = await _apiClient.GetAllAsync<UserReelInteraction>($"api/interactions/users/{userId}/reels/{reelId}");
            return results != null && results.Count > 0 ? results[0] : null;
        }

        public async Task<int> GetLikeCountAsync(int reelId)
        {
            var results = await _apiClient.GetAllAsync<int>($"api/interactions/reels/{reelId}/likes");
            return results != null && results.Count > 0 ? results[0] : 0;
        }

        public async Task<int?> GetReelMovieIdAsync(int reelId)
        {
            var results = await _apiClient.GetAllAsync<int?>($"api/interactions/reels/{reelId}/movie-id");
            return results != null && results.Count > 0 ? results[0] : null;
        }
    }
}