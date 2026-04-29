using System.Net;
using System.Net.Http.Json;
using MovieApp.WebApi.DTOs;

namespace MovieApp.Tests.Integration
{
    /// <summary>
    /// Integration tests for the ActiveSales WebAPI endpoints.
    /// </summary>
    public sealed class ActiveSalesEndpointsIntegrationTests : IClassFixture<MovieAppWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public ActiveSalesEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetCurrentSales_SeededDatabase_ReturnsOkStatusCode()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/active-sales/current");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrentSales_SeededDatabase_ReturnsNonEmptyList()
        {
            List<ActiveSaleDto>? currentSales = await _httpClient.GetFromJsonAsync<List<ActiveSaleDto>>("/api/active-sales/current");

            Assert.NotEmpty(currentSales!);
        }

        [Fact]
        public async Task GetCurrentSales_SeededDatabase_AllSalesHaveMovieReference()
        {
            List<ActiveSaleDto>? currentSales = await _httpClient.GetFromJsonAsync<List<ActiveSaleDto>>("/api/active-sales/current");

            bool allHaveMovieReference = currentSales!.All(sale => sale.Movie is not null);

            Assert.True(allHaveMovieReference);
        }

        [Fact]
        public async Task GetCurrentSales_SeededDatabase_AllSalesHavePositiveDiscount()
        {
            List<ActiveSaleDto>? currentSales = await _httpClient.GetFromJsonAsync<List<ActiveSaleDto>>("/api/active-sales/current");

            bool allHavePositiveDiscount = currentSales!.All(sale => sale.DiscountPercentage > 0);

            Assert.True(allHavePositiveDiscount);
        }
    }
}
