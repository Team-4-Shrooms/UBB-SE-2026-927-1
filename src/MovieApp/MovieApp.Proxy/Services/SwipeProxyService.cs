using System.Threading.Tasks;
using MovieApp.Logic.Features.MovieSwipe;

namespace MovieApp.Proxy.Services
{
    public class SwipeProxyService : ISwipeService
    {
        private readonly ApiClient _apiClient;

        public SwipeProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task UpdatePreferenceScoreAsync(int userId, int movieId, bool isLiked)
        {
            await _apiClient.PutAsync($"api/swipe/{userId}/preference", new { MovieId = movieId, IsLiked = isLiked });
        }
    }
}
