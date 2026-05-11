using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class EventProxyRepository : IEventRepository
    {
        private readonly ApiClient _apiClient;

        public EventProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<List<MovieEvent>> GetAllEventsAsync()
        {
            var result = await _apiClient.GetAsync<List<MovieEvent>>("api/events");
            return result ?? new List<MovieEvent>();
        }

        public async Task<MovieEvent?> GetEventByIdAsync(int eventId)
        {
            return await _apiClient.GetAsync<MovieEvent>($"api/events/{eventId}");
        }

        public async Task<List<MovieEvent>> GetEventsByMovieIdAsync(int movieId)
        {
            var result = await _apiClient.GetAsync<List<MovieEvent>>($"api/events/movie/{movieId}");
            return result ?? new List<MovieEvent>();
        }

        public async Task<bool> UserHasTicketAsync(int userId, int eventId)
        {
            return await _apiClient.GetAsync<bool>($"api/events/{eventId}/tickets/{userId}");
        }

        public async Task AddOwnedTicketAsync(OwnedTicket ticket)
        {
            await _apiClient.PostAsync("api/events/tickets", new
            {
                UserId = ticket.User?.Id ?? 0,
                EventId = ticket.Event?.Id ?? 0,
            });
        }

        public Task AddTransactionAsync(Transaction transaction) => Task.CompletedTask;

        public Task<int> SaveChangesAsync() => Task.FromResult(0);
    }
}
