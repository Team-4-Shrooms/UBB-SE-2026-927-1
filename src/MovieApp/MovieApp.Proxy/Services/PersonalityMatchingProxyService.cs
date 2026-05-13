using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.PersonalityMatch;

namespace MovieApp.Proxy.Services
{
    /// <summary>
    /// Proxy implementation of IPersonalityMatchingService.
    /// Delegates to the real PersonalityMatchingService backed by PersonalityMatchProxyRepository,
    /// so all cosine-similarity business logic is preserved while data fetching goes over HTTP.
    /// </summary>
    public class PersonalityMatchingProxyService : IPersonalityMatchingService
    {
        private readonly ApiClient _apiClient;

        public PersonalityMatchingProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<MatchResult>> GetTopMatchesAsync(int userId, int count)
        {
            return await _apiClient.GetAsync<List<MatchResult>>($"api/personality-match/users/{userId}/top-matches?count={count}") ?? new List<MatchResult>();
        }

        public async Task<List<MatchResult>> GetRandomUsersAsync(int userId, int count)
        {
            return await _apiClient.GetAsync<List<MatchResult>>($"api/personality-match/users/{userId}/random-users?count={count}") ?? new List<MatchResult>();
        }

        public async Task<UserProfile?> GetUserProfileAsync(int userId)
        {
            return await _apiClient.GetAsync<UserProfile>($"api/profiles/users/{userId}");
        }

        public async Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount)
        {
            return await _apiClient.GetAsync<List<MoviePreferenceDisplay>>($"api/personality-match/users/{userId}/top-preferences?count={topMoviePreferencesCount}") ?? new List<MoviePreferenceDisplay>();
        }

        public async Task<string> GetUsernameAsync(int userId)
        {
            return await _apiClient.GetAsync<string>($"api/personality-match/users/{userId}/username") ?? string.Empty;
        }
    }
}
