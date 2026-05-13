using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class MovieProxyService : IMovieService
    {
        private readonly ApiClient _apiClient;

        public MovieProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            // /api/movies has no list-all endpoint; scrape-jobs/movies exposes the full catalogue
            var result = await _apiClient.GetAsync<List<Movie>>("api/scrape-jobs/movies");
            return result ?? new List<Movie>();
        }

        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            return await _apiClient.GetAsync<Movie>($"api/movies/{id}");
        }

        public async Task<List<Movie>> SearchMoviesAsync(string? partialName)
        {
            var result = await _apiClient.GetAsync<List<Movie>>(
                $"api/movies/search?partialMovieName={Uri.EscapeDataString(partialName ?? string.Empty)}");
            return result ?? new List<Movie>();
        }

        public async Task<bool> UserOwnsMovieAsync(int userId, int movieId)
        {
            return await _apiClient.GetAsync<bool>($"api/movies/{movieId}/owned/{userId}");
        }

        public async Task PurchaseMovieAsync(int userId, int movieId, decimal price)
        {
            await _apiClient.PostAsync($"api/movies/{movieId}/purchase", new { UserId = userId, FinalPrice = price });
        }
    }
}
