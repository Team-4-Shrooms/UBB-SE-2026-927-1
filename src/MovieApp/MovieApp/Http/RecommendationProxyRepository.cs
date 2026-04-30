using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
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
            var results = await _apiClient.GetAllAsync<bool>($"api/recommendations/users/{userId}/has-preferences");
            return results.Count > 0 && results[0];
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            var results = await _apiClient.GetAllAsync<Reel>("api/recommendations/reels");
            return results;
        }

        public async Task<Dictionary<int, decimal>> GetUserPreferenceScoresAsync(int userId)
        {
            var results = await _apiClient.GetAllAsync<Dictionary<int, decimal>>($"api/recommendations/users/{userId}/preference-scores");
            return results.Count > 0 ? results[0] : new Dictionary<int, decimal>();
        }

        public async Task<Dictionary<int, int>> GetAllLikeCountsAsync()
        {
            var results = await _apiClient.GetAllAsync<Dictionary<int, int>>("api/recommendations/like-counts");
            return results.Count > 0 ? results[0] : new Dictionary<int, int>();
        }

        public async Task<List<UserReelInteraction>> GetLikesWithinDaysAsync(int days)
        {
            return await _apiClient.GetAllAsync<UserReelInteraction>($"api/recommendations/likes/within/{days}");
        }
    }
}