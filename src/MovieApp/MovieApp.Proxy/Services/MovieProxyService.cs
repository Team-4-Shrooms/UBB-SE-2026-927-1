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
            // Validate ownership
            bool alreadyOwned = await UserOwnsMovieAsync(userId, movieId);
            if (alreadyOwned)
                throw new InvalidOperationException("Movie already owned.");

            // Check balance
            decimal balance = await _apiClient.GetAsync<decimal>($"api/users/{userId}/balance");
            if (balance < price)
                throw new InvalidOperationException("Insufficient balance.");

            // Deduct balance
            await _apiClient.PutAsync($"api/users/{userId}/balance",
                new { NewBalance = balance - price });

            // Record ownership
            await _apiClient.PostAsync("api/inventory/ownedmovies",
                new { UserId = userId, MovieId = movieId });

            // Log transaction
            await _apiClient.PostAsync("api/transactions", new
            {
                Amount = -price,
                Type = "MoviePurchase",
                Status = "Completed",
                Timestamp = DateTime.UtcNow,
                BuyerId = userId,
                MovieId = movieId,
            });
        }
    }
}
