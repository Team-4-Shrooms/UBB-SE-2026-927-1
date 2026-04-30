using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
{
    public class ProfileProxyRepository : IProfileRepository
    {
        private readonly ApiClient _apiClient;

        public ProfileProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<UserProfile?> GetProfileAsync(int userId)
        {
            var results = await _apiClient.GetAllAsync<UserProfile>($"api/profiles/users/{userId}");
            return results.Count > 0 ? results[0] : null;
        }

        public async Task<List<UserReelInteraction>> GetInteractionsAsync(int userId)
        {
            return await _apiClient.GetAllAsync<UserReelInteraction>($"api/interactions/users/{userId}");
        }

        public async Task AddProfileAsync(UserProfile profile)
        {
            await _apiClient.PostAsync("api/profiles", profile);
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}