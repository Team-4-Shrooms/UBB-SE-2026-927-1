using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs;

namespace MovieApp.Logic.Http
{
    public class EquipmentProxyRepository : IEquipmentRepository
    {
        private readonly ApiClient _apiClient;

        public EquipmentProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Equipment>> FetchAvailableEquipmentAsync()
        {
            return await _apiClient.GetAllAsync<Equipment>("api/equipment/available");
        }

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            return await _apiClient.GetByIdAsync<Equipment>("api/equipment", id);
        }

        public async Task AddAsync(Equipment item)
        {
            await _apiClient.PostAsync("api/equipment", new EquipmentListItemRequestBody
            {
                Title = item.Title,
                Category = item.Category,
                Description = item.Description,
                Condition = item.Condition,
                Price = item.Price,
                ImageUrl = item.ImageUrl,
                SellerId = item.Seller?.Id ?? 0,
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
