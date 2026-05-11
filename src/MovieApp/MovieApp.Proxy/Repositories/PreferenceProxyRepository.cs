using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class PreferenceProxyRepository : IPreferenceRepository
    {
        private readonly ApiClient _apiClient;

        public PreferenceProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<bool> PreferenceExistsAsync(int userId, int movieId)
        {
            return await _apiClient.GetAsync<bool>($"api/preferences/users/{userId}/movies/{movieId}/exists");
        }

        public async Task InsertPreferenceAsync(int userId, int movieId, decimal score)
        {
            await _apiClient.PostAsync("api/preferences", new
            {
                UserId = userId,
                MovieId = movieId,
                Score = score,
            });
        }

        public async Task UpdatePreferenceAsync(int userId, int movieId, decimal boost)
        {
            await _apiClient.PutAsync($"api/preferences/users/{userId}/movies/{movieId}/boost", new
            {
                Boost = boost,
            });
        }

        public async Task<List<Movie>> GetMovieFeedAsync(int userId, int count)
        {
            var result = await _apiClient.GetAsync<List<Movie>>($"api/preferences/users/{userId}/feed?count={count}");
            return result ?? new List<Movie>();
        }
    }
}
