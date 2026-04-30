using System.Collections.Generic;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs;

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
            _apiClient.PostAsync("api/transactions", new LogTransactionRequestBody
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
            return _apiClient.GetAllAsync<Transaction>($"api/transactions/users/{userId}").GetAwaiter().GetResult();
        }

        public void UpdateTransactionStatus(int transactionId, string newStatus)
        {
            var payload = new UpdateTransactionStatusRequestBody { NewStatus = newStatus };
            _apiClient.PutAsync($"api/transactions/{transactionId}/status", payload).GetAwaiter().GetResult();
        }
    }
}
