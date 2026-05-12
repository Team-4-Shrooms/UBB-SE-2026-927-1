using System.Collections.Generic;
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
    }
}
