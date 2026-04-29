using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Http
{
    public class PersonalityMatchProxyRepository : IPersonalityMatchRepository
    {
        private readonly ApiClient _apiClient;

        public PersonalityMatchProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<UserMoviePreference>> GetAllPreferencesExceptUserAsync(int excludedUserId)
        {
            return await _apiClient.GetAllAsync<UserMoviePreference>($"api/personality-match/users/{excludedUserId}/others-preferences");
        }

        public async Task<List<UserMoviePreference>> GetCurrentUserPreferencesAsync(int userId)
        {
            return await _apiClient.GetAllAsync<UserMoviePreference>($"api/personality-match/users/{userId}/current-preferences");
        }

        public async Task<UserProfile?> GetUserProfileAsync(int userId)
        {
            return await _apiClient.GetAsync<UserProfile>($"api/personality-match/users/{userId}/profile");
        }

        public async Task<List<int>> GetRandomUserIdsAsync(int excludedUserId, int userIdsCount)
        {
            return await _apiClient.GetAllAsync<int>($"api/personality-match/users/{excludedUserId}/random-user-ids?userIdsCount={userIdsCount}");
        }
    }
}
