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
        private readonly RecommendationService _inner;

        public RecommendationProxyService(ApiClient apiClient)
        {
            _inner = new RecommendationService(new RecommendationProxyRepository(apiClient));
        }

        public Task<IList<Reel>> GetRecommendedReelsAsync(int userId, int count)
            => _inner.GetRecommendedReelsAsync(userId, count);

        //public Task<IList<Reel>> GetPersonalizedReelsAsync(int userid, int count)
        //    => _inner.GetPersonalizedReelsAsync(userid, count);
    }
}
