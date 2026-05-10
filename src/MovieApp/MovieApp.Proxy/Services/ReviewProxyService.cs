using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class ReviewProxyService : IReviewService
    {
        private readonly ApiClient _apiClient;

        public ReviewProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<int[]> GetStarRatingBucketsAsync(int movieId)
        {
            var result = await _apiClient.GetAsync<int[]>($"api/reviews/movie/{movieId}/buckets");
            return result ?? new int[11];
        }

        public async Task PostReviewAsync(int movieId, int userId, int rating, string? comment)
        {
            await _apiClient.PostAsync($"api/reviews/movie/{movieId}", new { UserId = userId, Rating = rating, Comment = comment });
        }

        public async Task<List<MovieReview>> GetReviewsForMovieAsync(int movieId)
        {
            var result = await _apiClient.GetAsync<List<MovieReview>>($"api/reviews/movie/{movieId}");
            return result ?? new List<MovieReview>();
        }
    }
}
