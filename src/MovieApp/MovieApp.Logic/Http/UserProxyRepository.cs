using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

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
            return _apiClient.GetAsync<decimal>($"api/users/{userId}/balance").GetAwaiter().GetResult();
        }

        public void UpdateBalance(int userId, decimal newBalance)
        {
            var payload = new { newBalance };
            _apiClient.PutAsync($"api/users/{userId}/balance", payload).GetAwaiter().GetResult();
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}
