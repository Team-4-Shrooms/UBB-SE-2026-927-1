using System.Threading.Tasks;
using MovieApp.Logic.Features.MovieSwipe;

namespace MovieApp.Proxy.Services
{
    /// <summary>
    /// Proxy implementation of ISwipeService.
    /// Replicates SwipeService logic (check-then-insert-or-update) via preference endpoints.
    /// </summary>
    public class SwipeProxyService : ISwipeService
    {
        private const double LikeDelta = 1.0;
        private const double SkipDelta = -0.5;

        private readonly ApiClient _apiClient;

        public SwipeProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task UpdatePreferenceScoreAsync(int userId, int movieId, bool isLiked)
        {
            await _apiClient.PostAsync("api/swipe", new { UserId = userId, MovieId = movieId, IsLiked = isLiked });
        }
    }
}
