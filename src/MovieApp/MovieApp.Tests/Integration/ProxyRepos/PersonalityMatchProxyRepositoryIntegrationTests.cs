using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class PersonalityMatchProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task GetCurrentUserPreferencesAsync_UserWithInsertedPreference_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        PreferenceProxyRepository preferenceRepository = new PreferenceProxyRepository(testContext.ApiClient);
        PersonalityMatchProxyRepository personalityMatchRepository = new PersonalityMatchProxyRepository(testContext.ApiClient);

        await preferenceRepository.InsertPreferenceAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, 7.5m);

        List<UserMoviePreference> preferences = await personalityMatchRepository.GetCurrentUserPreferencesAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.NotEmpty(preferences);
    }
}
