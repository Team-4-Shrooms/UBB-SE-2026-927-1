using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class UserProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task GetUserByIdAsync_SeededUser_ReturnsMatchingId()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        UserProxyRepository userRepository = new UserProxyRepository(testContext.ApiClient);

        MovieApp.DataLayer.Models.User? user = await userRepository.GetUserByIdAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal(ProxyRepoSeedIds.SeededUserId, user?.Id);
    }

    [Fact]
    public async Task UpdateBalanceAsync_ExistingUser_ChangesBalance()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        UserProxyRepository userRepository = new UserProxyRepository(testContext.ApiClient);

        await userRepository.UpdateBalanceAsync(ProxyRepoSeedIds.SeededUserId, 123.45m);
        decimal updatedBalance = await userRepository.GetBalanceAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal(123.45m, updatedBalance);
    }
}
