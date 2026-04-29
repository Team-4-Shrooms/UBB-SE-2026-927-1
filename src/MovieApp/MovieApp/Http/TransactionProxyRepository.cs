using System.Collections.Generic;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
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
            _apiClient.PostAsync("api/transactions", transaction).GetAwaiter().GetResult();
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