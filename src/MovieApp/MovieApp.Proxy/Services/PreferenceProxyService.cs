using System.Threading.Tasks;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class PreferenceProxyService : IPreferenceService
    {
        private readonly ApiClient _apiClient;

        public PreferenceProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> PreferenceExistsAsync(int userId, int movieId)
        {
            return await _apiClient.GetAsync<bool>($"api/preferences/users/{userId}/movies/{movieId}/exists");
        }

        public async Task InsertPreferenceAsync(int userId, int movieId, decimal score)
        {
            await _apiClient.PostAsync("api/preferences", new { UserId = userId, MovieId = movieId, Score = score });
        }

        public async Task UpdatePreferenceAsync(int userId, int movieId, decimal boost)
        {
            await _apiClient.PutAsync($"api/preferences/users/{userId}/movies/{movieId}/boost", new { Boost = boost });
        }
    }
}
