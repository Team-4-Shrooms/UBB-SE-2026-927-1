using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Services;

namespace MovieApp.Proxy.Services
{
    /// <summary>
    /// Proxy implementation of IPersonalityMatchService.
    /// Delegates to the real PersonalityMatchService backed by proxy repositories,
    /// so business logic is preserved while data fetching goes over HTTP.
    /// </summary>
    public class PersonalityMatchProxyService : IPersonalityMatchService
    {
        private readonly ApiClient _apiClient;

        public PersonalityMatchProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Dictionary<int, List<UserMoviePreference>>> GetAllPreferencesGroupedAsync(int excludedUserId)
        {
            // The WebAPI returns a flat list, the service groups it.
            // But wait, the controller for "others-preferences" returns flat list.
            // Let's check the controller.
            var result = await _apiClient.GetAsync<List<UserMoviePreference>>($"api/personality-match/users/{excludedUserId}/others-preferences");
            return result?
                .GroupBy(p => p.User.Id)
                .ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<UserMoviePreference>>();
        }

        public async Task<string> GetUsernameAsync(int userId)
        {
            return await _apiClient.GetAsync<string>($"api/personality-match/users/{userId}/username") ?? string.Empty;
        }

        public async Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount)
        {
            return await _apiClient.GetAsync<List<MoviePreferenceDisplay>>($"api/personality-match/users/{userId}/top-preferences?count={topMoviePreferencesCount}") ?? new List<MoviePreferenceDisplay>();
        }

        public async Task<List<UserMoviePreference>> GetCurrentUserPreferencesAsync(int userId)
        {
            return await _apiClient.GetAsync<List<UserMoviePreference>>($"api/personality-match/users/{userId}/current-preferences") ?? new List<UserMoviePreference>();
        }

        public async Task<List<int>> GetRandomUserIdsAsync(int excludedUserId, int userIdsCount)
        {
            return await _apiClient.GetAsync<List<int>>($"api/personality-match/users/{excludedUserId}/random-user-ids?userIdsCount={userIdsCount}") ?? new List<int>();
        }
    }
}
