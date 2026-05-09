using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class EventProxyService : IEventService
    {
        private readonly ApiClient _apiClient;

        public EventProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<MovieEvent>> GetAvailableEventsAsync()
        {
            var result = await _apiClient.GetAsync<List<MovieEvent>>("api/events/available");
            return result ?? new List<MovieEvent>();
        }

        public async Task PurchaseTicketAsync(int userId, int eventId)
        {
            await _apiClient.PostAsync($"api/events/{eventId}/purchase?userId={userId}", new { });
        }

        public async Task<MovieEvent?> GetEventByIdAsync(int id)
        {
            return await _apiClient.GetAsync<MovieEvent>($"api/events/{id}");
        }

        public async Task<bool> UserHasTicketAsync(int userId, int eventId)
        {
            return await _apiClient.GetAsync<bool>($"api/events/{eventId}/hasticket?userId={userId}");
        }
    }
}
