using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class EquipmentProxyRepository : IEquipmentRepository
    {
        private readonly ApiClient _apiClient;

        public EquipmentProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<List<Equipment>> FetchAvailableEquipmentAsync()
        {
            var result = await _apiClient.GetAsync<List<Equipment>>("api/equipment/available");
            return result ?? new List<Equipment>();
        }

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            return await _apiClient.GetAsync<Equipment>($"api/equipment/{id}");
        }

        public async Task AddAsync(Equipment item)
        {
            await _apiClient.PostAsync("api/equipment", new
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

        public Task AddTransactionAsync(Transaction transaction) => Task.CompletedTask;

        public Task<int> SaveChangesAsync() => Task.FromResult(0);
    }
}
