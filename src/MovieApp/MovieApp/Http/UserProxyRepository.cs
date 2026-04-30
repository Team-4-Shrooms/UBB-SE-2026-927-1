using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
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
            // Guessed Endpoint: GET /api/users/{id}
            // If this fails, try "api/profiles/users" 
            var results = await _apiClient.GetAllAsync<User>($"api/users/{id}");
            return results.Count > 0 ? results[0] : null;
        }

        public decimal GetBalance(int userId)
        {
            var results = _apiClient.GetAllAsync<decimal>($"api/users/{userId}/balance").GetAwaiter().GetResult();
            return results.Count > 0 ? results[0] : 0m;
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