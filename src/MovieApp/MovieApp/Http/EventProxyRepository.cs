using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
{
    public class EventProxyRepository : IEventRepository
    {
        private readonly ApiClient _apiClient;

        public EventProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<MovieEvent>> GetEventsByMovieIdAsync(int movieId)
        {
            return await _apiClient.GetAllAsync<MovieEvent>($"api/events/movie/{movieId}");
        }

        public async Task<List<MovieEvent>> GetAllEventsAsync()
        {
            return await _apiClient.GetAllAsync<MovieEvent>("api/events");
        }

        public async Task<MovieEvent?> GetEventByIdAsync(int eventId)
        {
            return await _apiClient.GetByIdAsync<MovieEvent>("api/events", eventId);
        }

        public async Task<bool> UserHasTicketAsync(int userId, int eventId)
        {
            var results = await _apiClient.GetAllAsync<bool>($"api/events/{eventId}/tickets/{userId}");
            return results.Count > 0 && results[0];
        }

        public async Task AddOwnedTicketAsync(OwnedTicket ticket)
        {
            await _apiClient.PostAsync("api/events/tickets", ticket);
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