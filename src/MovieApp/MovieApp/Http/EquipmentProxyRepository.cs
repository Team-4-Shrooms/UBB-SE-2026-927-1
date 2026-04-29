using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
{
    public class EquipmentProxyRepository : IEquipmentRepository
    {
        private readonly ApiClient _apiClient;

        public EquipmentProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public List<Equipment> FetchAvailableEquipment()
        {
            return _apiClient.GetAllAsync<Equipment>("api/equipment/available").GetAwaiter().GetResult();
        }

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            return await _apiClient.GetByIdAsync<Equipment>("api/equipment", id);
        }

        public async Task AddAsync(Equipment item)
        {
            await _apiClient.PostAsync("api/equipment", item);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _apiClient.PostAsync("api/transactions", transaction);
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}