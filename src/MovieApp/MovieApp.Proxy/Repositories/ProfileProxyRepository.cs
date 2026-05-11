using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class ProfileProxyRepository : IProfileRepository
    {
        private readonly ApiClient _apiClient;

        public ProfileProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<UserProfile?> GetProfileAsync(int userId)
        {
            return await _apiClient.GetAsync<UserProfile>($"api/profiles/users/{userId}");
        }

        public Task<List<UserReelInteraction>> GetInteractionsAsync(int userId) =>
            Task.FromResult(new List<UserReelInteraction>());

        public async Task AddProfileAsync(UserProfile profile)
        {
            await _apiClient.PostAsync("api/profiles", new
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

        public Task<int> SaveChangesAsync() => Task.FromResult(0);
    }
}
