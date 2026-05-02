using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Logic.Http
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
            return await _apiClient.GetAsync<UserProfile>($"api/profiles/users/{userId}");
        }

        public async Task<List<UserReelInteraction>> GetInteractionsAsync(int userId)
        {
            return await _apiClient.GetAllAsync<UserReelInteraction>($"api/interactions/users/{userId}");
        }

        public async Task AddProfileAsync(UserProfile profile)
        {
            await _apiClient.PostAsync("api/profiles", new UpsertProfileRequestBody
            {
                TotalLikes = profile.TotalLikes,
                TotalWatchTimeSeconds = profile.TotalWatchTimeSeconds,
                AverageWatchTimeSeconds = profile.AverageWatchTimeSeconds,
                TotalClipsViewed = profile.TotalClipsViewed,
                LikeToViewRatio = profile.LikeToViewRatio,
                LastUpdated = profile.LastUpdated,
                UserId = profile.User?.Id ?? 0,
            });
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}
