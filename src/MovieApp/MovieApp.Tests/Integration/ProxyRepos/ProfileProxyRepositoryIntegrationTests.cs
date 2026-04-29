using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ProfileProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task AddProfileAsync_ExistingUser_MakesProfileRetrievable()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        ProfileProxyRepository profileRepository = new ProfileProxyRepository(testContext.ApiClient);

        await profileRepository.AddProfileAsync(new UserProfile
        {
            TotalLikes = 12,
            TotalWatchTimeSeconds = 3600,
            AverageWatchTimeSeconds = 42.5m,
            TotalClipsViewed = 18,
            LikeToViewRatio = 0.66m,
            LastUpdated = DateTime.UtcNow,
            User = new User { Id = ProxyRepoSeedIds.SeededUserId },
        });

        UserProfile? profile = await profileRepository.GetProfileAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal(ProxyRepoSeedIds.SeededUserId, profile?.User?.Id);
    }
}
