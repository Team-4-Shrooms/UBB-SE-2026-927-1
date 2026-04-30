using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Http
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
            return await _apiClient.GetAsync<bool>($"api/events/{eventId}/tickets/{userId}");
        }

        public async Task AddOwnedTicketAsync(OwnedTicket ticket)
        {
            await _apiClient.PostAsync("api/events/tickets", (object)new
            {
                userId = ticket.User?.Id ?? 0,
                eventId = ticket.Event?.Id ?? 0,
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
