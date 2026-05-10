using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.PersonalityMatch;

namespace MovieApp.Proxy.Services
{
    public class PersonalityMatchingProxyService : IPersonalityMatchingService
    {
        private readonly ApiClient _apiClient;

        public PersonalityMatchingProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<MatchResult>> GetTopMatchesAsync(int userId, int count)
        {
            var result = await _apiClient.GetAsync<List<MatchResult>>($"api/personalitymatching/{userId}/matches?count={count}");
            return result ?? new List<MatchResult>();
        }

        public async Task<List<MatchResult>> GetRandomUsersAsync(int userId, int count)
        {
            var result = await _apiClient.GetAsync<List<MatchResult>>($"api/personalitymatching/{userId}/random?count={count}");
            return result ?? new List<MatchResult>();
        }

        public async Task<UserProfile?> GetUserProfileAsync(int userId)
        {
            return await _apiClient.GetAsync<UserProfile>($"api/personalitymatching/{userId}/profile");
        }

        public async Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount)
        {
            var result = await _apiClient.GetAsync<List<MoviePreferenceDisplay>>($"api/personalitymatching/{userId}/preferences?count={topMoviePreferencesCount}");
            return result ?? new List<MoviePreferenceDisplay>();
        }

        public async Task<string> GetUsernameAsync(int userId)
        {
            var result = await _apiClient.GetAsync<string>($"api/personalitymatching/{userId}/name");
            return result ?? string.Empty;
        }
    }
}
