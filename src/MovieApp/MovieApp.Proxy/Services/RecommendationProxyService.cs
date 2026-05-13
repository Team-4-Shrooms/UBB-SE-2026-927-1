using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.ReelsFeed;

namespace MovieApp.Proxy.Services
{
    /// <summary>
    /// Proxy implementation of IRecommendationService.
    /// Delegates to the real RecommendationService backed by RecommendationProxyRepository,
    /// so the personalized/cold-start ranking logic is preserved while data goes over HTTP.
    /// </summary>
    public class RecommendationProxyService : IRecommendationService
    {
        private readonly ApiClient _apiClient;

        public RecommendationProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IList<Reel>> GetRecommendedReelsAsync(int userId, int count)
        {
            return await _apiClient.GetAsync<IList<Reel>>($"api/recommendations/users/{userId}/recommended-reels/count={count}");
        }

        public async Task<bool> UserHasPreferencesAsync(int userId)
        {
            return await _apiClient.GetAsync<bool>($"api/recommendations/users/{userId}/has-preferences");
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            return await _apiClient.GetAsync<IList<Reel>>("api/recommendations/reels");
        }

        public async Task<IDictionary<int, decimal>> GetUserPreferenceScoresAsync(int userId)
        {
            return await _apiClient.GetAsync<IDictionary<int, decimal>>($"api/recommendations/users/{userId}/preference-scores");
        }

        public async Task<IDictionary<int, int>> GetAllLikeCountsAsync()
        {
            return await _apiClient.GetAsync<IDictionary<int, int>>("api/recommendations/like-counts");
        }

        public async Task<IList<UserReelInteraction>> GetLikesWithinDaysAsync(int days)
        {
            return await _apiClient.GetAsync<IList<UserReelInteraction>>($"api/recommendations/likes/within/{days}");
        }
    }
}
