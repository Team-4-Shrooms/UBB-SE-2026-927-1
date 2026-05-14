using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class EventProxyServiceIntegrationTests
{
    [Fact]
    public async Task GetAvailableEventsAsync_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EventProxyService eventRepository = new EventProxyService(testContext.ApiClient);

        List<MovieEvent> allEvents = await eventRepository.GetAvailableEventsAsync();

        Assert.NotEmpty(allEvents);
    }

    [Fact]
    public async Task PurchaseTicketAsync_ExistingUserAndEvent_SetsUserHasTicketTrue()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EventProxyService eventRepository = new EventProxyService(testContext.ApiClient);
        List<MovieEvent> allEvents = await eventRepository.GetAvailableEventsAsync();
        int eventId = allEvents[0].Id;

        await eventRepository.PurchaseTicketAsync(ProxyRepoSeedIds.SeededUserId, eventId);

        bool userHasTicket = await eventRepository.UserHasTicketAsync(ProxyRepoSeedIds.SeededUserId, eventId);

        Assert.True(userHasTicket);
    }
}

