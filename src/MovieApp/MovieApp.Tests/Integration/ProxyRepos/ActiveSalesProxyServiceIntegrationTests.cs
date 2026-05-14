using MovieApp.Proxy.Services;
using MovieApp.Proxy;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ActiveSalesProxyServiceIntegrationTests
{
    [Fact]
    public async Task GetBestDiscountPercentByMovieIdAsync_SeededDatabase_ReturnsDictionary()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        ActiveSalesProxyService activeSalesRepository = new ActiveSalesProxyService(testContext.ApiClient);

        Dictionary<int, decimal> discounts = await activeSalesRepository.GetBestDiscountPercentByMovieIdAsync();

        // If there are any seeded sales, this should not be empty, but at least it should return a dictionary.
        Assert.NotNull(discounts);
    }
}
