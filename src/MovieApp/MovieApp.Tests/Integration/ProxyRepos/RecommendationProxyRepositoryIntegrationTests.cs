using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class RecommendationProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task GetAllReelsAsync_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        RecommendationProxyRepository recommendationRepository = new RecommendationProxyRepository(testContext.ApiClient);

        IList<Reel> allReels = await recommendationRepository.GetAllReelsAsync();

        Assert.NotEmpty(allReels);
    }

    [Fact]
    public async Task UserHasPreferencesAsync_UserWithInsertedPreference_ReturnsTrue()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        PreferenceProxyRepository preferenceRepository = new PreferenceProxyRepository(testContext.ApiClient);
        RecommendationProxyRepository recommendationRepository = new RecommendationProxyRepository(testContext.ApiClient);

        await preferenceRepository.InsertPreferenceAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, 6.5m);
        bool userHasPreferences = await recommendationRepository.UserHasPreferencesAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.True(userHasPreferences);
    }
}
