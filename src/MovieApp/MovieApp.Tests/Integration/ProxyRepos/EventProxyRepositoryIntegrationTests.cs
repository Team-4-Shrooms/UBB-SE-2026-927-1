using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class EventProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task GetAllEventsAsync_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EventProxyRepository eventRepository = new EventProxyRepository(testContext.ApiClient);

        List<MovieEvent> allEvents = await eventRepository.GetAllEventsAsync();

        Assert.NotEmpty(allEvents);
    }

    [Fact]
    public async Task AddOwnedTicketAsync_ExistingUserAndEvent_SetsUserHasTicketTrue()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EventProxyRepository eventRepository = new EventProxyRepository(testContext.ApiClient);
        List<MovieEvent> allEvents = await eventRepository.GetAllEventsAsync();
        int eventId = allEvents[0].Id;

        await eventRepository.AddOwnedTicketAsync(new OwnedTicket
        {
            User = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Event = new MovieEvent { Id = eventId },
        });

        bool userHasTicket = await eventRepository.UserHasTicketAsync(ProxyRepoSeedIds.SeededUserId, eventId);

        Assert.True(userHasTicket);
    }
}
