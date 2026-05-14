using MovieApp.Proxy.Services;
using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class PreferenceProxyServiceIntegrationTests
{
    [Fact]
    public async Task InsertPreferenceAsync_ExistingUserAndMovie_MarksPreferenceAsExisting()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        PreferenceProxyService preferenceRepository = new PreferenceProxyService(testContext.ApiClient);

        await preferenceRepository.InsertPreferenceAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, 8.5m);
        bool preferenceExists = await preferenceRepository.PreferenceExistsAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId);

        Assert.True(preferenceExists);
    }
}




