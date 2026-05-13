using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class ProfileProxyService : IProfileService
    {
        private readonly ApiClient _apiClient;

        public ProfileProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<UserProfile> BuildProfileFromInteractionsAsync(int userId)
        {
            var result = await _apiClient.GetAsync<UserProfile>($"api/profiles/users/{userId}");
            return result ?? new UserProfile();
        }

        public async Task<decimal> GetUserBalanceAsync(int userId)
        {
            return await _apiClient.GetAsync<decimal>($"api/users/{userId}/balance");
        }

        public async Task<List<Transaction>> GetUserTransactionsAsync(int userId, int page, int pageSize)
        {
            var result = await _apiClient.GetAsync<List<Transaction>>($"api/transactions/users/{userId}?page={page}&pageSize={pageSize}");
            return result ?? new List<Transaction>();
        }
    }
}
