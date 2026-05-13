using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class RecommendationProxyRepository : IRecommendationRepository
    {
        private readonly ApiClient _apiClient;

        public RecommendationProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<bool> UserHasPreferencesAsync(int userId)
        {
            return await _apiClient.GetAsync<bool>($"api/recommendations/users/{userId}/has-preferences");
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            //[HttpGet("users/{userId:int}/recommended-reels/count={n:int}")]
            var result = await _apiClient.GetAsync<List<Reel>>("api/recommendations/reels");
            return result ?? new List<Reel>();
        }

        public async Task<IList<Reel>> GetPersonalizedReelsAsync(int userId, int count)
        {
            var result = await _apiClient.GetAsync<List<Reel>>($"api/recommendations/users/{userId}/recommended-reels/count={count}");
            return result ?? new List<Reel>();
        }

        public async Task<Dictionary<int, decimal>> GetUserPreferenceScoresAsync(int userId)
        {
            var result = await _apiClient.GetAsync<Dictionary<int, decimal>>($"api/recommendations/users/{userId}/preference-scores");
            return result ?? new Dictionary<int, decimal>();
        }

        public async Task<Dictionary<int, int>> GetAllLikeCountsAsync()
        {
            var result = await _apiClient.GetAsync<Dictionary<int, int>>("api/recommendations/like-counts");
            return result ?? new Dictionary<int, int>();
        }

        public async Task<List<UserReelInteraction>> GetLikesWithinDaysAsync(int days)
        {
            var result = await _apiClient.GetAsync<List<UserReelInteraction>>($"api/recommendations/likes/within/{days}");
            return result ?? new List<UserReelInteraction>();
        }
    }
}
