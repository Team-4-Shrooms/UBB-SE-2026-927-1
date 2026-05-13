using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class MovieTournamentProxyServiceIntegrationTests
{
    [Fact]
    public async Task BoostMovieScoreAsync_BoostedPreference_AddsMovieToTournamentPool()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        PreferenceProxyService preferenceRepository = new PreferenceProxyService(testContext.ApiClient);
        MovieTournamentProxyService movieTournamentRepository = new MovieTournamentProxyService(testContext.ApiClient);

        await preferenceRepository.InsertPreferenceAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, 5m);
        await movieTournamentRepository.BoostMovieScoreAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, 2m);

        List<Movie> tournamentPool = await movieTournamentRepository.GetTournamentPoolAsync(ProxyRepoSeedIds.SeededUserId, 10);

        Assert.NotEmpty(tournamentPool);
    }
}




