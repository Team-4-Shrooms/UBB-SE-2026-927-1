using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class ReviewProxyRepository : IReviewRepository
    {
        private readonly ApiClient _apiClient;

        public ReviewProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<List<MovieReview>> GetReviewsForMovieAsync(int movieId)
        {
            var result = await _apiClient.GetAsync<List<MovieReview>>($"api/reviews/movie/{movieId}");
            return result ?? new List<MovieReview>();
        }

        public async Task<List<decimal>> GetRawRatingsForMovieAsync(int movieId)
        {
            var result = await _apiClient.GetAsync<List<decimal>>($"api/reviews/movie/{movieId}/ratings");
            return result ?? new List<decimal>();
        }

        public async Task<Dictionary<int, int>> GetReviewCountsAsync(IEnumerable<int> movieIds)
        {
            var result = await _apiClient.PostAsync<object, Dictionary<int, int>>("api/reviews/counts", new
            {
                MovieIds = movieIds,
            });
            return result ?? new Dictionary<int, int>();
        }

        public async Task AddReviewAsync(MovieReview review)
        {
            await _apiClient.PostAsync("api/reviews", new
            {
                MovieId = review.Movie?.Id ?? 0,
                UserId = review.User?.Id ?? 0,
                StarRating = (int)review.StarRating,
                Comment = review.Comment,
            });
        }

        public Task<int> SaveChangesAsync() => Task.FromResult(0);
    }
}
