using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class EquipmentProxyServiceIntegrationTests
{
    [Fact]
    public async Task GetAvailableEquipmentAsync_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EquipmentProxyService equipmentRepository = new EquipmentProxyService(testContext.ApiClient);

        List<Equipment> available = await equipmentRepository.GetAvailableEquipmentAsync();

        Assert.NotEmpty(available);
    }

    [Fact]
    public async Task ListItemAsync_NewItem_MakesItAvailable()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EquipmentProxyService equipmentRepository = new EquipmentProxyService(testContext.ApiClient);
        string uniqueTitle = $"Unit Test Camera {Guid.NewGuid():N}";

        await equipmentRepository.ListItemAsync(new Equipment
        {
            Title = uniqueTitle,
            Description = "Proxy repository integration test",
            Price = 2500m,
            Condition = "Excellent",
            Status = EquipmentStatus.Available,
            Seller = new User { Id = ProxyRepoSeedIds.SeededUserId },
        });

        List<Equipment> available = await equipmentRepository.GetAvailableEquipmentAsync();

        Assert.True(available.Any(item => item.Title == uniqueTitle));
    }
}
