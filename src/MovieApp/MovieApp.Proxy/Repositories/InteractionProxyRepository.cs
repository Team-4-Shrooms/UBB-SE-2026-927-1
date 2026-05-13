using System.Diagnostics;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class InteractionProxyRepository : IInteractionRepository
    {
        private readonly ApiClient _apiClient;
        public InteractionProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task InsertInteractionAsync(UserReelInteraction interaction)
        {
            await _apiClient.PostAsync("api/interactions", new
            {
                IsLiked = interaction.IsLiked,
                WatchDurationSeconds = interaction.WatchDurationSeconds,
                WatchPercentage = interaction.WatchPercentage,
                ViewedAt = interaction.ViewedAt,
                UserId = interaction.User?.Id ?? 0,
                ReelId = interaction.Reel?.Id ?? 0,
            });
        }

        public async Task UpsertInteractionAsync(int userId, int reelId)
        {
            await _apiClient.PostAsync($"api/interactions/users/{userId}/reels/{reelId}", new { });
        }

        public async Task ToggleLikeAsync(int userId, int reelId)
        {
            Debug.WriteLine($"liked in proxyrepo");
            await _apiClient.PutAsync($"api/interactions/users/{userId}/reels/{reelId}/like", new { });
        }

        public async Task UpdateViewDataAsync(int userId, int reelId, decimal watchDurationSeconds, decimal watchPercentage)
        {
            await _apiClient.PutAsync($"api/interactions/users/{userId}/reels/{reelId}/view", new
            {
                WatchDurationSeconds = watchDurationSeconds,
                WatchPercentage = watchPercentage,
            });
        }

        public async Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId)
        {
            try
            {
                return await _apiClient.GetAsync<UserReelInteraction>($"api/interactions/users/{userId}/reels/{reelId}");
            }
            catch (Exception ex) when (ex is System.Text.Json.JsonException || ex is HttpRequestException)
            {
                return null;
            }
        }

        public async Task<int> GetLikeCountAsync(int reelId)
        {
            return await _apiClient.GetAsync<int>($"api/interactions/reels/{reelId}/likes");
        }

        public async Task<int?> GetReelMovieIdAsync(int reelId)
        {
            return await _apiClient.GetAsync<int?>($"api/interactions/reels/{reelId}/movie-id");
        }
    }
}
