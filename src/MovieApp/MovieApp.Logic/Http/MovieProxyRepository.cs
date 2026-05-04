using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Logic.Http
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
            return await _apiClient.GetAsync<bool>($"api/movies/{movieId}/owned/{userId}");
        }

        public async Task AddOwnedMovieAsync(OwnedMovie ownership)
        {
            await _apiClient.PostAsync("api/inventory/ownedmovies", new AddOwnedMovieRequestBody
            {
                UserId = ownership.User?.Id ?? 0,
                MovieId = ownership.Movie?.Id ?? 0,
            });
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _apiClient.PostAsync("api/transactions", new LogTransactionRequestBody
            {
                Amount = transaction.Amount,
                Type = transaction.Type,
                Status = transaction.Status,
                Timestamp = transaction.Timestamp,
                ShippingAddress = transaction.ShippingAddress,
                BuyerId = transaction.Buyer?.Id ?? 0,
                SellerId = transaction.Seller?.Id,
                EquipmentId = transaction.Equipment?.Id,
                MovieId = transaction.Movie?.Id,
                EventId = transaction.Event?.Id,
            });
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}
