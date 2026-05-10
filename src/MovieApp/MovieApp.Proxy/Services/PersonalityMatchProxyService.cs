using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class PersonalityMatchProxyService : IPersonalityMatchService
    {
        private readonly ApiClient _apiClient;

        public PersonalityMatchProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Dictionary<int, List<UserMoviePreference>>> GetAllPreferencesGroupedAsync(int excludedUserId)
        {
            var result = await _apiClient.GetAsync<Dictionary<int, List<UserMoviePreference>>>($"api/personalitymatch/preferences?exclude={excludedUserId}");
            return result ?? new Dictionary<int, List<UserMoviePreference>>();
        }

        public async Task<string> GetUsernameAsync(int userId)
        {
            var result = await _apiClient.GetAsync<string>($"api/personalitymatch/user/{userId}/name");
            return result ?? string.Empty;
        }

        public async Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount)
        {
            var result = await _apiClient.GetAsync<List<MoviePreferenceDisplay>>($"api/personalitymatch/user/{userId}/top-preferences?count={topMoviePreferencesCount}");
            return result ?? new List<MoviePreferenceDisplay>();
        }
    }
}
