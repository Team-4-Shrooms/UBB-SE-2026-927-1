using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class MovieTournamentProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task BoostMovieScoreAsync_BoostedPreference_AddsMovieToTournamentPool()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        PreferenceProxyRepository preferenceRepository = new PreferenceProxyRepository(testContext.ApiClient);
        MovieTournamentProxyRepository movieTournamentRepository = new MovieTournamentProxyRepository(testContext.ApiClient);

        await preferenceRepository.InsertPreferenceAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, 5m);
        await movieTournamentRepository.BoostMovieScoreAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, 2m);

        List<Movie> tournamentPool = await movieTournamentRepository.GetTournamentPoolAsync(ProxyRepoSeedIds.SeededUserId, 10);

        Assert.NotEmpty(tournamentPool);
    }
}
