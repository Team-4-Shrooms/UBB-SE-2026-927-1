using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
{
    public class MovieProxyRepository : IMovieRepository
    {
        private readonly ApiClient _apiClient;

        public MovieProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Movie?> GetMovieByIdAsync(int movieId)
        {
            return await _apiClient.GetByIdAsync<Movie>("api/movies", movieId);
        }

        public async Task<List<Movie>> SearchMoviesAsync(string query, int limit)
        {
            return await _apiClient.GetAllAsync<Movie>($"api/movies/search?partialMovieName={query}&limit={limit}");
        }

        public async Task<bool> UserOwnsMovieAsync(int userId, int movieId)
        {
            var results = await _apiClient.GetAllAsync<bool>($"api/movies/{movieId}/owned/{userId}");
            return results.Count > 0 && results[0];
        }

        public async Task AddOwnedMovieAsync(OwnedMovie ownership)
        {
            await _apiClient.PostAsync("api/inventory/ownedmovies", ownership);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _apiClient.PostAsync("api/transactions", transaction);
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}