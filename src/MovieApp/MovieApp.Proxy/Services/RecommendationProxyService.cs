using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.ReelsFeed;

namespace MovieApp.Proxy.Services
{
    public class RecommendationProxyService : IRecommendationService
    {
        private readonly ApiClient _apiClient;

        public RecommendationProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IList<Reel>> GetRecommendedReelsAsync(int userId, int count)
        {
            var result = await _apiClient.GetAsync<List<Reel>>($"api/recommendations/{userId}?count={count}");
            return result ?? new List<Reel>();
        }
    }
}
