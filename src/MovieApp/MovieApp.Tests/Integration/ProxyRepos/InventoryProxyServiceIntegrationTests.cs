using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class InventoryProxyServiceIntegrationTests
{
    [Fact]
    public async Task RemoveMovieOwnerships_ExistingOwnership_RemovesOwnershipRecord()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        MovieProxyService movieRepository = new MovieProxyService(testContext.ApiClient);
        InventoryProxyService inventoryRepository = new InventoryProxyService(testContext.ApiClient);
        Movie movie = await movieRepository.GetMovieByIdAsync(ProxyRepoSeedIds.SeededMovieId)
            ?? throw new InvalidOperationException("Seeded movie was not found.");

        await inventoryRepository.AddOwnedMovieAsync(ProxyRepoSeedIds.SeededUserId, movie.Id);

        List<OwnedMovie> ownerships = await inventoryRepository.GetMovieOwnershipsAsync(ProxyRepoSeedIds.SeededUserId, movie.Id);
        await inventoryRepository.RemoveMovieOwnershipsAsync(ownerships.Select(o => o.Id));

        List<OwnedMovie> remainingOwnerships = await inventoryRepository.GetMovieOwnershipsAsync(ProxyRepoSeedIds.SeededUserId, movie.Id);

        Assert.Empty(remainingOwnerships);
    }

    [Fact]
    public async Task RemoveTicketOwnerships_ExistingOwnership_RemovesOwnershipRecord()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EventProxyService eventRepository = new EventProxyService(testContext.ApiClient);
        InventoryProxyService inventoryRepository = new InventoryProxyService(testContext.ApiClient);
        List<MovieEvent> allEvents = await eventRepository.GetAvailableEventsAsync();
        int eventId = allEvents[0].Id;

        await eventRepository.PurchaseTicketAsync(ProxyRepoSeedIds.SeededUserId, eventId);

        List<OwnedTicket> ownerships = await inventoryRepository.GetTicketOwnershipsAsync(ProxyRepoSeedIds.SeededUserId, eventId);
        await inventoryRepository.RemoveTicketOwnershipsAsync(ownerships.Select(o => o.Id));

        List<OwnedTicket> remainingOwnerships = await inventoryRepository.GetTicketOwnershipsAsync(ProxyRepoSeedIds.SeededUserId, eventId);

        Assert.Empty(remainingOwnerships);
    }
}
