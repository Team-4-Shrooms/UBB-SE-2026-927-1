using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Logic.Http
{
    public class ReviewProxyRepository : IReviewRepository
    {
        private readonly ApiClient _apiClient;

        public ReviewProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<MovieReview>> GetReviewsForMovieAsync(int movieId)
        {
            return await _apiClient.GetAllAsync<MovieReview>($"api/reviews/movie/{movieId}");
        }

        public async Task<List<decimal>> GetRawRatingsForMovieAsync(int movieId)
        {
            return await _apiClient.GetAllAsync<decimal>($"api/reviews/movie/{movieId}/ratings");
        }

        public async Task<Dictionary<int, int>> GetReviewCountsAsync(IEnumerable<int> movieIds)
        {
            var results = await _apiClient.PostAsync<Dictionary<int, int>>("api/reviews/counts", new GetReviewCountsRequestBody { MovieIds = movieIds });
            return results ?? new Dictionary<int, int>();
        }

        public async Task AddReviewAsync(MovieReview review)
        {
            await _apiClient.PostAsync("api/reviews", new AddReviewRequestBody
            {
                MovieId = review.Movie?.Id ?? 0,
                UserId = review.User?.Id ?? 0,
                StarRating = (int)review.StarRating,
                Comment = review.Comment,
            });
        }

        public Task<int> SaveChangesAsync() => Task.FromResult(1);
    }
}
