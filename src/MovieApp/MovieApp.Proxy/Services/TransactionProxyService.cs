using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class TransactionProxyService : ITransactionService
    {
        private readonly ApiClient _apiClient;

        public TransactionProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Transaction>> GetTransactionsAsync(int page, int pageSize)
        {
            var result = await _apiClient.GetAsync<List<Transaction>>($"api/transactions?page={page}&pageSize={pageSize}");
            return result ?? new List<Transaction>();
        }

        public async Task<List<Transaction>> GetTransactionsByUserIdAsync(int userId, int? page = null, int? pageSize = null)
        {
            var result = await _apiClient.GetAsync<List<Transaction>>($"api/transactions/users/{userId}?page={page}&pageSize={pageSize}");
            return result ?? new List<Transaction>();
        }

        public async Task LogTransactionAsync(Transaction transaction)
        {
            await _apiClient.PostAsync("api/transactions", new
            {
                Amount = transaction.Amount,
                Type = transaction.Type,
                Status = transaction.Status,
                Timestamp = transaction.Timestamp,
                ShippingAddress = transaction.ShippingAddress,
                BuyerId = transaction.Buyer?.Id,
                SellerId = transaction.Seller?.Id,
                EquipmentId = transaction.Equipment?.Id,
                MovieId = transaction.Movie?.Id,
                EventId = transaction.Event?.Id
            });
        }

        public async Task UpdateTransactionStatusAsync(int transactionId, string status)
        {
            await _apiClient.PutAsync($"api/transactions/{transactionId}/status", new { NewStatus = status });
        }
    }
}
