using System.Collections.Generic;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Http
{
    public class TransactionProxyRepository : ITransactionRepository
    {
        private readonly ApiClient _apiClient;

        public TransactionProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public void LogTransaction(Transaction transaction)
        {
            _apiClient.PostAsync("api/transactions", (object)new
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
            }).GetAwaiter().GetResult();
        }

        public List<Transaction> GetTransactionsByUserId(int userId)
        {
            return _apiClient.GetAllAsync<Transaction>($"api/transactions/users/{userId}").GetAwaiter().GetResult();
        }

        public void UpdateTransactionStatus(int transactionId, string newStatus)
        {
            var payload = new { newStatus };
            _apiClient.PutAsync($"api/transactions/{transactionId}/status", payload).GetAwaiter().GetResult();
        }
    }
}
