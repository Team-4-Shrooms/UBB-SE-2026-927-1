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
    public void UpdateBalance_ExistingUser_ChangesBalance()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        UserProxyRepository userRepository = new UserProxyRepository(testContext.ApiClient);

        userRepository.UpdateBalance(ProxyRepoSeedIds.SeededUserId, 123.45m);
        decimal updatedBalance = userRepository.GetBalance(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal(123.45m, updatedBalance);
    }
}
