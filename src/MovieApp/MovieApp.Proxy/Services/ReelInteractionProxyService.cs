using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.ReelsFeed;
using MovieApp.Proxy;

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
            await _apiClient.PutAsync<object>($"api/interactions/users/{userId}/reels/{reelId}/like", null!);
        }

        public async Task<int> GetLikeCountAsync(int reelId)
        {
            return await _apiClient.GetAsync<int>($"api/interactions/reels/{reelId}/likes");
        }

        public async Task RecordViewAsync(int userId, int reelId, double watchDurationSec, double watchPercentage)
        {
            await _apiClient.PutAsync($"api/interactions/users/{userId}/reels/{reelId}/view", new { WatchDurationSeconds = watchDurationSec, WatchPercentage = watchPercentage });
        }

        public async Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId)
        {
            return await _apiClient.GetAsync<UserReelInteraction>($"api/interactions/users/{userId}/reels/{reelId}");
        }

        public async Task InsertInteractionAsync(UserReelInteraction interaction)
        {
            await _apiClient.PostAsync("api/interactions", new { UserId = interaction.User.Id, ReelId = interaction.Reel.Id, IsLiked = interaction.IsLiked, WatchDurationSeconds = interaction.WatchDurationSeconds, WatchPercentage = interaction.WatchPercentage, ViewedAt = interaction.ViewedAt });
        }

        public async Task UpsertInteractionAsync(int userId, int reelId)
        {
            await _apiClient.PostAsync<object>($"api/interactions/users/{userId}/reels/{reelId}", null!);
        }

        public async Task<int> GetReelMovieIdAsync(int reelId)
        {
            return await _apiClient.GetAsync<int>($"api/interactions/reels/{reelId}/movie-id");
        }

        public async Task<IList<UserReelInteraction>> GetInteractionsForUserAsync(int userId)
        {
            return await _apiClient.GetAsync<IList<UserReelInteraction>>($"api/interactions/users/{userId}");
        }
    }
}
