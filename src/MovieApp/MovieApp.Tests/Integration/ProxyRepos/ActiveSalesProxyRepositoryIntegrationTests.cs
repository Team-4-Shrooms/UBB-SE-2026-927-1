using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ActiveSalesProxyRepositoryIntegrationTests
{
    [Fact]
    public void GetCurrentSales_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        ActiveSalesProxyRepository activeSalesRepository = new ActiveSalesProxyRepository(testContext.ApiClient);

        List<MovieApp.DataLayer.Models.ActiveSale> currentSales = activeSalesRepository.GetCurrentSales();

        Assert.NotEmpty(currentSales);
    }
}
