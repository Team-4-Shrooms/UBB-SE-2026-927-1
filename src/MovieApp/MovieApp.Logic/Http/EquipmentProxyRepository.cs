using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

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
            await _apiClient.PostAsync("api/equipment", (object)new
            {
                title = item.Title,
                category = item.Category,
                description = item.Description,
                condition = item.Condition,
                price = item.Price,
                imageUrl = item.ImageUrl,
                sellerId = item.Seller?.Id ?? 0,
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
