using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Xunit;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class MovieProxyServiceIntegrationTests
{
    [Fact]
    public async Task GetMovieByIdAsync_SeededMovie_ReturnsMatchingId()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        MovieProxyService movieRepository = new MovieProxyService(testContext.ApiClient);

        Movie? movie = await movieRepository.GetMovieByIdAsync(ProxyRepoSeedIds.SeededMovieId);

        Assert.Equal(ProxyRepoSeedIds.SeededMovieId, movie?.Id);
    }

    [Fact]
    public async Task PurchaseMovieAsync_ExistingUserAndMovie_SetsUserOwnsMovieTrue()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        MovieProxyService movieRepository = new MovieProxyService(testContext.ApiClient);
        Movie movie = await movieRepository.GetMovieByIdAsync(ProxyRepoSeedIds.SeededMovieId)
            ?? throw new InvalidOperationException("Seeded movie was not found.");

        await movieRepository.PurchaseMovieAsync(ProxyRepoSeedIds.SeededUserId, movie.Id, 10.0m);

        bool ownsMovie = await movieRepository.UserOwnsMovieAsync(ProxyRepoSeedIds.SeededUserId, movie.Id);

        Assert.True(ownsMovie);
    }
}
