using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
{
    public class InventoryProxyRepository : IInventoryRepository
    {
        private readonly ApiClient _apiClient;

        public InventoryProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Movie>> GetOwnedMoviesAsync(int userId)
        {
            return await _apiClient.GetAllAsync<Movie>($"api/inventory/users/{userId}/movies");
        }

        public async Task<List<OwnedMovie>> GetMovieOwnershipsAsync(int userId, int movieId)
        {
            return await _apiClient.GetAllAsync<OwnedMovie>($"api/inventory/users/{userId}/movies/{movieId}/ownerships");
        }

        public async void RemoveMovieOwnerships(IEnumerable<OwnedMovie> ownerships)
        {
            await _apiClient.PostAsync("api/inventory/movies/ownerships/remove", ownerships);
        }

        public async Task<List<OwnedTicket>> GetTicketOwnershipsAsync(int userId, int eventId)
        {
            return await _apiClient.GetAllAsync<OwnedTicket>($"api/inventory/users/{userId}/events/{eventId}/tickets");
        }

        public async void RemoveTicketOwnerships(IEnumerable<OwnedTicket> ownerships)
        {
            await _apiClient.PostAsync("api/inventory/events/tickets/remove", ownerships);
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