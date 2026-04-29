using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
{
    public class PreferenceProxyRepository : IPreferenceRepository
    {
        private readonly ApiClient _apiClient;

        public PreferenceProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> PreferenceExistsAsync(int userId, int movieId)
        {
            var results = await _apiClient.GetAllAsync<bool>($"api/preferences/users/{userId}/movies/{movieId}/exists");
            return results.Count > 0 && results[0];
        }

        public async Task InsertPreferenceAsync(int userId, int movieId, decimal score)
        {
            var payload = new { userId, movieId, score };
            await _apiClient.PostAsync("api/preferences", payload);
        }

        public async Task UpdatePreferenceAsync(int userId, int movieId, decimal boost)
        {
            var payload = new { boost };
            await _apiClient.PutAsync($"api/preferences/users/{userId}/movies/{movieId}/boost", payload);
        }
    }
}