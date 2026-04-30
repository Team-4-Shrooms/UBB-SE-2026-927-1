using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

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
            await _apiClient.PostAsync("api/inventory/ownedmovies", (object)new
            {
                userId = ownership.User?.Id ?? 0,
                movieId = ownership.Movie?.Id ?? 0,
            });
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _apiClient.PostAsync("api/transactions", (object)new
            {
                amount = transaction.Amount,
                type = transaction.Type,
                status = transaction.Status,
                timestamp = transaction.Timestamp,
                shippingAddress = transaction.ShippingAddress,
                buyerId = transaction.Buyer?.Id ?? 0,
                sellerId = transaction.Seller?.Id,
                equipmentId = transaction.Equipment?.Id,
                movieId = transaction.Movie?.Id,
                eventId = transaction.Event?.Id,
            });
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}
