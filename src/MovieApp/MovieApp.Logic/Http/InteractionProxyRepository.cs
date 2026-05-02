using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Logic.Http
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
            await _apiClient.PostAsync("api/interactions", new InsertInteractionRequestBody
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
            await _apiClient.PostAsync($"api/interactions/users/{userId}/reels/{reelId}", (object)new { });
        }

        public async Task ToggleLikeAsync(int userId, int reelId)
        {
            await _apiClient.PutAsync($"api/interactions/users/{userId}/reels/{reelId}/like", new { });
        }

        public async Task UpdateViewDataAsync(int userId, int reelId, decimal watchDurationSeconds, decimal watchPercentage)
        {
            var payload = new UpdateViewDataRequestBody { WatchDurationSeconds = watchDurationSeconds, WatchPercentage = watchPercentage };
            await _apiClient.PutAsync($"api/interactions/users/{userId}/reels/{reelId}/view", payload);
        }

        public async Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId)
        {
            return await _apiClient.GetAsync<UserReelInteraction>($"api/interactions/users/{userId}/reels/{reelId}");
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
