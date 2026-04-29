using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Http
{
    public class RecommendationProxyRepository : IRecommendationRepository
    {
        private readonly ApiClient _apiClient;

        public RecommendationProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> UserHasPreferencesAsync(int userId)
        {
            return await _apiClient.GetAsync<bool>($"api/recommendations/users/{userId}/has-preferences");
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            return await _apiClient.GetAllAsync<Reel>("api/recommendations/reels");
        }

        public async Task<Dictionary<int, decimal>> GetUserPreferenceScoresAsync(int userId)
        {
            return await _apiClient.GetAsync<Dictionary<int, decimal>>($"api/recommendations/users/{userId}/preference-scores") ?? new Dictionary<int, decimal>();
        }

        public async Task<Dictionary<int, int>> GetAllLikeCountsAsync()
        {
            return await _apiClient.GetAsync<Dictionary<int, int>>("api/recommendations/like-counts") ?? new Dictionary<int, int>();
        }

        public async Task<List<UserReelInteraction>> GetLikesWithinDaysAsync(int days)
        {
            return await _apiClient.GetAllAsync<UserReelInteraction>($"api/recommendations/likes/within/{days}");
        }
    }
}
