using System.Net;
using System.Net.Http.Json;
using MovieApp.WebDTOs.DTOs;

namespace MovieApp.Tests.Integration
{
    /// <summary>
    /// Integration tests for the Equipment WebAPI endpoints.
    /// </summary>
    public sealed class EquipmentEndpointsIntegrationTests : IClassFixture<MovieAppWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public EquipmentEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetAvailableEquipment_SeededDatabase_ReturnsOkStatusCode()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/equipment/available");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAvailableEquipment_SeededDatabase_ReturnsNonEmptyList()
        {
            List<EquipmentDto>? availableEquipment = await _httpClient.GetFromJsonAsync<List<EquipmentDto>>("/api/equipment/available");

            Assert.NotEmpty(availableEquipment!);
        }

        [Fact]
        public async Task GetAvailableEquipment_SeededDatabase_AllItemsHaveSellerReference()
        {
            List<EquipmentDto>? availableEquipment = await _httpClient.GetFromJsonAsync<List<EquipmentDto>>("/api/equipment/available");

            bool allHaveSellerReference = availableEquipment!.All(equipment => equipment.Seller is not null);

            Assert.True(allHaveSellerReference);
        }

        [Fact]
        public async Task GetAvailableEquipment_SeededDatabase_AllItemsHaveNonEmptyTitle()
        {
            List<EquipmentDto>? availableEquipment = await _httpClient.GetFromJsonAsync<List<EquipmentDto>>("/api/equipment/available");

            bool allHaveNonEmptyTitle = availableEquipment!.All(equipment => !string.IsNullOrWhiteSpace(equipment.Title));

            Assert.True(allHaveNonEmptyTitle);
        }
    }
}
