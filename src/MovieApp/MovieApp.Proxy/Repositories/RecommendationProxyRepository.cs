using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.WriteLine("userhaspreferences");
            var result = await _apiClient.GetAsync<bool>($"api/recommendations/users/{userId}/has-preferences");
            Debug.WriteLine(result);
            return result;
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            //[HttpGet("users/{userId:int}/recommended-reels/count={n:int}")]
            Debug.WriteLine("debugging");
            var result = await _apiClient.GetAsync<List<Reel>>("api/recommendations/reels");
            return result ?? new List<Reel>();
        }

        public async Task<IList<Reel>> GetPersonalizedReelsAsync(int userId, int count)
        {
            Debug.WriteLine("debugging1");
            var result = await _apiClient.GetAsync<List<Reel>>($"api/recommendations/users/{userId}/recommended-reels/count={count}");
            return result ?? new List<Reel>();
        }

        public async Task<Dictionary<int, decimal>> GetUserPreferenceScoresAsync(int userId)
        {
            Debug.WriteLine("getpreferencescores");
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
