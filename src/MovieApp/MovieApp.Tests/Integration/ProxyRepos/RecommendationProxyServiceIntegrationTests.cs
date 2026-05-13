using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class RecommendationProxyServiceIntegrationTests
{
    [Fact]
    public async Task GetAllReelsAsync_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        RecommendationProxyService recommendationRepository = new RecommendationProxyService(testContext.ApiClient);

        IList<Reel> allReels = await recommendationRepository.GetAllReelsAsync();

        Assert.NotEmpty(allReels);
    }

    [Fact]
    public async Task UserHasPreferencesAsync_UserWithInsertedPreference_ReturnsTrue()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        PreferenceProxyService preferenceRepository = new PreferenceProxyService(testContext.ApiClient);
        RecommendationProxyService recommendationRepository = new RecommendationProxyService(testContext.ApiClient);

        await preferenceRepository.InsertPreferenceAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, 6.5m);
        bool userHasPreferences = await recommendationRepository.UserHasPreferencesAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.True(userHasPreferences);
    }
}




