using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class PreferenceProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task InsertPreferenceAsync_ExistingUserAndMovie_MarksPreferenceAsExisting()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        PreferenceProxyRepository preferenceRepository = new PreferenceProxyRepository(testContext.ApiClient);

        await preferenceRepository.InsertPreferenceAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, 8.5m);
        bool preferenceExists = await preferenceRepository.PreferenceExistsAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId);

        Assert.True(preferenceExists);
    }
}
