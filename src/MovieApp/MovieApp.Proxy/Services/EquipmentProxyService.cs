using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class EquipmentProxyService : IEquipmentService
    {
        private readonly ApiClient _apiClient;

        public EquipmentProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task ListItemAsync(Equipment item)
        {
            var body = new
            {
                Title = item.Title,
                Category = item.Category,
                Description = item.Description,
                Condition = item.Condition,
                Price = item.Price,
                ImageUrl = item.ImageUrl,
                SellerId = item.Seller.Id
            };
            await _apiClient.PostAsync("api/equipment", body);
        }

        public async Task PurchaseEquipmentAsync(int equipmentId, int buyerId, decimal price, string address)
        {
            await _apiClient.PostAsync($"api/equipment/{equipmentId}/purchase", new { buyerId, price, address });
        }

        public async Task<List<Equipment>> GetAvailableEquipmentAsync()
        {
            var result = await _apiClient.GetAsync<List<Equipment>>("api/equipment/available");
            return result ?? new List<Equipment>();
        }

        public async Task<Equipment?> GetEquipmentByIdAsync(int id)
        {
            return await _apiClient.GetAsync<Equipment>($"api/equipment/{id}");
        }
    }
}
