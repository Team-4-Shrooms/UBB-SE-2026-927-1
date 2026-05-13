using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class UserProxyService : IUserService
    {
        private readonly ApiClient _apiClient;

        public UserProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _apiClient.GetAsync<User>($"api/users/{userId}");
        }

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            return await _apiClient.GetAsync<decimal>($"api/users/{userId}/balance");
        }

        public async Task UpdateBalanceAsync(int userId, decimal newBalance)
        {
            await _apiClient.PutAsync($"api/users/{userId}/balance", new { NewBalance = newBalance });
        }
    }
}
