using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Http
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
            return await _apiClient.GetAsync<bool>($"api/preferences/users/{userId}/movies/{movieId}/exists");
        }

        public async Task InsertPreferenceAsync(int userId, int movieId, decimal score)
        {
            var payload = new { userId, movieId, score };
            await _apiClient.PostAsync("api/preferences", (object)payload);
        }

        public async Task UpdatePreferenceAsync(int userId, int movieId, decimal boost)
        {
            var payload = new { boost };
            await _apiClient.PutAsync($"api/preferences/users/{userId}/movies/{movieId}/boost", payload);
        }
    }
}