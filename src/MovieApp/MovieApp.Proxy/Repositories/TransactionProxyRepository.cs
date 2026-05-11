using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class TransactionProxyRepository : ITransactionRepository
    {
        private readonly ApiClient _apiClient;

        public TransactionProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public void LogTransaction(Transaction transaction)
        {
            _apiClient.PostAsync("api/transactions", new
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
            }).GetAwaiter().GetResult();
        }

        public List<Transaction> GetTransactionsByUserId(int userId)
        {
            var result = _apiClient.GetAsync<List<Transaction>>($"api/transactions/users/{userId}").GetAwaiter().GetResult();
            return result ?? new List<Transaction>();
        }

        public void UpdateTransactionStatus(int transactionId, string newStatus)
        {
            _apiClient.PutAsync($"api/transactions/{transactionId}/status", new
            {
                NewStatus = newStatus,
            }).GetAwaiter().GetResult();
        }
    }
}
