using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Logic.Http
{
    public class UserProxyRepository : IUserRepository
    {
        private readonly ApiClient _apiClient;

        public UserProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _apiClient.GetAsync<User>($"api/users/{id}");
        }

        public decimal GetBalance(int userId)
        {
            try
            {
                return _apiClient.GetAsync<decimal>($"api/users/{userId}/balance").GetAwaiter().GetResult();
            }
            catch
            {
                return 0m;
            }
        }

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            try
            {
                return await _apiClient.GetAsync<decimal>($"api/users/{userId}/balance");
            }
            catch
            {
                return 0m;
            }
        }

        public void UpdateBalance(int userId, decimal newBalance)
        {
            try
            {
                var payload = new UpdateBalanceRequestBody { NewBalance = newBalance };
                _apiClient.PutAsync($"api/users/{userId}/balance", payload).GetAwaiter().GetResult();
            }
            catch
            {
                // Silently fail for now, but ideally we should log this
            }
        }

        public async Task UpdateBalanceAsync(int userId, decimal newBalance)
        {
            try
            {
                var payload = new UpdateBalanceRequestBody { NewBalance = newBalance };
                await _apiClient.PutAsync($"api/users/{userId}/balance", payload);
            }
            catch
            {
                // Silently fail for now, but ideally we should log this
            }
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}
