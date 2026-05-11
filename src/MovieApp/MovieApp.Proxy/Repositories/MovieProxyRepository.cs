using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class MovieProxyRepository : IMovieRepository
    {
        private readonly ApiClient _apiClient;

        public MovieProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<Movie?> GetMovieByIdAsync(int movieId)
        {
            return await _apiClient.GetAsync<Movie>($"api/movies/{movieId}");
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            // No dedicated list-all endpoint; use the scrape-jobs movies endpoint
            var result = await _apiClient.GetAsync<List<Movie>>("api/scrape-jobs/movies");
            return result ?? new List<Movie>();
        }

        public async Task<List<Movie>> SearchMoviesAsync(string query, int limit)
        {
            var result = await _apiClient.GetAsync<List<Movie>>($"api/movies/search?partialMovieName={Uri.EscapeDataString(query)}");
            return result ?? new List<Movie>();
        }

        public async Task<bool> UserOwnsMovieAsync(int userId, int movieId)
        {
            return await _apiClient.GetAsync<bool>($"api/movies/{movieId}/owned/{userId}");
        }

        public async Task AddOwnedMovieAsync(OwnedMovie ownership)
        {
            await _apiClient.PostAsync("api/inventory/ownedmovies", new
            {
                UserId = ownership.User?.Id ?? 0,
                MovieId = ownership.Movie?.Id ?? 0,
            });
        }

        public Task AddTransactionAsync(Transaction transaction) => Task.CompletedTask;

        public Task<int> SaveChangesAsync() => Task.FromResult(0);
    }
}
