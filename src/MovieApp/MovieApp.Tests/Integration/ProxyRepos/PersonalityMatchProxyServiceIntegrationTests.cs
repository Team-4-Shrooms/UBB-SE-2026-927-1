using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class PersonalityMatchProxyServiceIntegrationTests
{
    [Fact]
    public async Task GetCurrentUserPreferencesAsync_UserWithInsertedPreference_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        PreferenceProxyService preferenceRepository = new PreferenceProxyService(testContext.ApiClient);
        PersonalityMatchProxyService personalityMatchRepository = new PersonalityMatchProxyService(testContext.ApiClient);

        await preferenceRepository.InsertPreferenceAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, 7.5m);

        List<UserMoviePreference> preferences = await personalityMatchRepository.GetCurrentUserPreferencesAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.NotEmpty(preferences);
    }
}




