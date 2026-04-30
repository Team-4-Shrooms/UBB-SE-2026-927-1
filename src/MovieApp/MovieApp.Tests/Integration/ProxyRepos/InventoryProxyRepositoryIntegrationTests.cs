using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class InventoryProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task RemoveMovieOwnerships_ExistingOwnership_RemovesOwnershipRecord()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        MovieProxyRepository movieRepository = new MovieProxyRepository(testContext.ApiClient);
        InventoryProxyRepository inventoryRepository = new InventoryProxyRepository(testContext.ApiClient);
        Movie movie = await movieRepository.GetMovieByIdAsync(ProxyRepoSeedIds.SeededMovieId)
            ?? throw new InvalidOperationException("Seeded movie was not found.");

        await movieRepository.AddOwnedMovieAsync(new OwnedMovie
        {
            User = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = movie.Id },
        });

        List<OwnedMovie> ownerships = await inventoryRepository.GetMovieOwnershipsAsync(ProxyRepoSeedIds.SeededUserId, movie.Id);
        await inventoryRepository.RemoveMovieOwnershipsAsync(ownerships);

        List<OwnedMovie> remainingOwnerships = await inventoryRepository.GetMovieOwnershipsAsync(ProxyRepoSeedIds.SeededUserId, movie.Id);

        Assert.Empty(remainingOwnerships);
    }

    [Fact]
    public async Task RemoveTicketOwnerships_ExistingOwnership_RemovesOwnershipRecord()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EventProxyRepository eventRepository = new EventProxyRepository(testContext.ApiClient);
        InventoryProxyRepository inventoryRepository = new InventoryProxyRepository(testContext.ApiClient);
        List<MovieEvent> allEvents = await eventRepository.GetAllEventsAsync();
        int eventId = allEvents[0].Id;

        await eventRepository.AddOwnedTicketAsync(new OwnedTicket
        {
            User = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Event = new MovieEvent { Id = eventId },
        });

        List<OwnedTicket> ownerships = await inventoryRepository.GetTicketOwnershipsAsync(ProxyRepoSeedIds.SeededUserId, eventId);
        await inventoryRepository.RemoveTicketOwnershipsAsync(ownerships);

        List<OwnedTicket> remainingOwnerships = await inventoryRepository.GetTicketOwnershipsAsync(ProxyRepoSeedIds.SeededUserId, eventId);

        Assert.Empty(remainingOwnerships);
    }
}
