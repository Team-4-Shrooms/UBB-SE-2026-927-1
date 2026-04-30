using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Http
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

        public async Task RemoveMovieOwnershipsAsync(IEnumerable<OwnedMovie> ownerships)
        {
            var ids = ownerships.Select(o => o.Id).ToList();
            await _apiClient.PostAsync("api/inventory/movies/ownerships/remove", (object)ids);
        }

        public async Task<List<OwnedTicket>> GetTicketOwnershipsAsync(int userId, int eventId)
        {
            return await _apiClient.GetAllAsync<OwnedTicket>($"api/inventory/users/{userId}/events/{eventId}/tickets");
        }

        public async Task RemoveTicketOwnershipsAsync(IEnumerable<OwnedTicket> ownerships)
        {
            var ids = ownerships.Select(o => o.Id).ToList();
            await _apiClient.PostAsync("api/inventory/events/tickets/remove", (object)ids);
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
