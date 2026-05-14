using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;
using System.Threading.Tasks;
using System;
using Xunit;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ProfileProxyServiceIntegrationTests
{
    [Fact]
    public async Task AddProfileAsync_ExistingUser_MakesProfileRetrievable()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        ProfileProxyService profileRepository = new ProfileProxyService(testContext.ApiClient);

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

        UserProfile? profile = await profileRepository.BuildProfileFromInteractionsAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal(ProxyRepoSeedIds.SeededUserId, profile?.User?.Id);
    }
}

