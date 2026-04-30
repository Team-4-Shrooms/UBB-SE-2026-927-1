using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ActiveSalesProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task GetCurrentSalesAsync_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        ActiveSalesProxyRepository activeSalesRepository = new ActiveSalesProxyRepository(testContext.ApiClient);

        List<MovieApp.DataLayer.Models.ActiveSale> currentSales = await activeSalesRepository.GetCurrentSalesAsync();

        Assert.NotEmpty(currentSales);
    }
}
