using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs;

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
            await _apiClient.PostAsync("api/events/tickets", new AddOwnedTicketRequestBody
            {
                UserId = ticket.User?.Id ?? 0,
                EventId = ticket.Event?.Id ?? 0,
            });
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _apiClient.PostAsync("api/transactions", new LogTransactionRequestBody
            {
                Amount = transaction.Amount,
                Type = transaction.Type,
                Status = transaction.Status,
                Timestamp = transaction.Timestamp,
                ShippingAddress = transaction.ShippingAddress,
                BuyerId = transaction.Buyer?.Id ?? 0,
                SellerId = transaction.Seller?.Id,
                EquipmentId = transaction.Equipment?.Id,
                MovieId = transaction.Movie?.Id,
                EventId = transaction.Event?.Id,
            });
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}
