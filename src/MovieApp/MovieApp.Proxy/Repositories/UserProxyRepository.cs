using System;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class UserProxyRepository : IUserRepository
    {
        private readonly ApiClient _apiClient;

        public UserProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _apiClient.GetAsync<User>($"api/users/{id}");
        }

        public Task<User?> GetUserByUsernameAsync(string username) =>
            throw new NotImplementedException("GetUserByUsernameAsync is not available via the proxy.");

        public Task<int> SaveChangesAsync() => Task.FromResult(0);

        public decimal GetBalance(int userId) => GetBalanceAsync(userId).GetAwaiter().GetResult();

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            return await _apiClient.GetAsync<decimal>($"api/users/{userId}/balance");
        }

        public void UpdateBalance(int userId, decimal newBalance) =>
            UpdateBalanceAsync(userId, newBalance).GetAwaiter().GetResult();

        public async Task UpdateBalanceAsync(int userId, decimal newBalance)
        {
            await _apiClient.PutAsync($"api/users/{userId}/balance", new
            {
                NewBalance = newBalance,
            });
        }
    }
}
