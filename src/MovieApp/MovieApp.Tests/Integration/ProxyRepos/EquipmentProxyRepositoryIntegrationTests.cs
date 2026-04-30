using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class EquipmentProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task FetchAvailableEquipment_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EquipmentProxyRepository equipmentRepository = new EquipmentProxyRepository(testContext.ApiClient);

        List<Equipment> availableEquipment = equipmentRepository.FetchAvailableEquipment();

        await Task.CompletedTask;
        Assert.NotEmpty(availableEquipment);
    }

    [Fact]
    public async Task AddAsync_NewEquipment_MakesEquipmentVisibleInAvailableList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EquipmentProxyRepository equipmentRepository = new EquipmentProxyRepository(testContext.ApiClient);

        string uniqueTitle = $"Proxy Equipment {Guid.NewGuid():N}";
        await equipmentRepository.AddAsync(new Equipment
        {
            Title = uniqueTitle,
            Category = "Lighting",
            Description = "Proxy repository integration test equipment",
            Condition = "New",
            Price = 49.99m,
            ImageUrl = "https://example.com/equipment.png",
            Seller = new User { Id = ProxyRepoSeedIds.SeededUserId },
        });

        List<Equipment> availableEquipment = equipmentRepository.FetchAvailableEquipment();

        Assert.True(availableEquipment.Any(equipment => equipment.Title == uniqueTitle));
    }
}
