using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class InventoryProxyService : IInventoryService
    {
        private readonly ApiClient _apiClient;

        public InventoryProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task RemoveOwnedMovieAsync(int userId, int movieId)
        {
            await _apiClient.PostAsync($"api/inventory/remove-movie", new { userId, movieId });
        }

        public async Task RemoveOwnedTicketAsync(int userId, int eventId)
        {
            await _apiClient.PostAsync($"api/inventory/remove-ticket", new { userId, eventId });
        }

        public async Task<List<Movie>> GetOwnedMoviesAsync(int userId)
        {
            var result = await _apiClient.GetAsync<List<Movie>>($"api/inventory/users/{userId}/movies");
            return result ?? new List<Movie>();
        }

        public async Task<List<OwnedTicket>> GetOwnedTicketsAsync(int userId)
        {
            var result = await _apiClient.GetAsync<List<OwnedTicket>>($"api/inventory/users/{userId}/tickets");
            return result ?? new List<OwnedTicket>();
        }

        public async Task<List<OwnedMovie>> GetMovieOwnershipsAsync(int userId, int movieId)
        {
            var result = await _apiClient.GetAsync<List<OwnedMovie>>($"api/inventory/users/{userId}/movies/{movieId}/ownerships");
            return result ?? new List<OwnedMovie>();
        }

        public async Task RemoveMovieOwnershipsAsync(IEnumerable<int> ownershipIds)
        {
            await _apiClient.PostAsync("api/inventory/movies/ownerships/remove", ownershipIds.ToList());
        }

        public async Task<List<OwnedTicket>> GetTicketOwnershipsAsync(int userId, int eventId)
        {
            var result = await _apiClient.GetAsync<List<OwnedTicket>>($"api/inventory/users/{userId}/events/{eventId}/tickets");
            return result ?? new List<OwnedTicket>();
        }

        public async Task RemoveTicketOwnershipsAsync(IEnumerable<int> ownershipIds)
        {
            await _apiClient.PostAsync("api/inventory/events/tickets/remove", ownershipIds.ToList());
        }

        public async Task AddOwnedMovieAsync(int userId, int movieId)
        {
            await _apiClient.PostAsync("api/inventory/ownedmovies", new { userId, movieId });
        }
    }
}
