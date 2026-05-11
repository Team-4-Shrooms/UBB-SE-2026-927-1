using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class InventoryProxyRepository : IInventoryRepository
    {
        private readonly ApiClient _apiClient;

        public InventoryProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<List<Movie>> GetOwnedMoviesAsync(int userId)
        {
            var result = await _apiClient.GetAsync<List<Movie>>($"api/inventory/users/{userId}/movies");
            return result ?? new List<Movie>();
        }

        public async Task<List<OwnedMovie>> GetMovieOwnershipsAsync(int userId, int movieId)
        {
            var result = await _apiClient.GetAsync<List<OwnedMovie>>($"api/inventory/users/{userId}/movies/{movieId}/ownerships");
            return result ?? new List<OwnedMovie>();
        }

        public async Task RemoveMovieOwnershipsAsync(IEnumerable<OwnedMovie> ownerships)
        {
            var ids = ownerships.Select(o => o.Id).ToList();
            await _apiClient.PostAsync("api/inventory/movies/ownerships/remove", ids);
        }

        public async Task<List<OwnedTicket>> GetTicketOwnershipsAsync(int userId, int eventId)
        {
            var result = await _apiClient.GetAsync<List<OwnedTicket>>($"api/inventory/users/{userId}/events/{eventId}/tickets");
            return result ?? new List<OwnedTicket>();
        }

        public async Task RemoveTicketOwnershipsAsync(IEnumerable<OwnedTicket> ownerships)
        {
            var ids = ownerships.Select(o => o.Id).ToList();
            await _apiClient.PostAsync("api/inventory/events/tickets/remove", ids);
        }

        public Task<List<OwnedTicket>> GetAllTicketsForUserAsync(int userId) =>
            Task.FromResult(new List<OwnedTicket>());

        public Task AddTransactionAsync(Transaction transaction) => Task.CompletedTask;

        public Task<int> SaveChangesAsync() => Task.FromResult(0);
    }
}
