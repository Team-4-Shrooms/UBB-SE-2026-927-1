using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class MovieProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task GetMovieByIdAsync_SeededMovie_ReturnsMatchingId()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        MovieProxyRepository movieRepository = new MovieProxyRepository(testContext.ApiClient);

        Movie? movie = await movieRepository.GetMovieByIdAsync(ProxyRepoSeedIds.SeededMovieId);

        Assert.Equal(ProxyRepoSeedIds.SeededMovieId, movie?.Id);
    }

    [Fact]
    public async Task AddOwnedMovieAsync_ExistingUserAndMovie_SetsUserOwnsMovieTrue()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        MovieProxyRepository movieRepository = new MovieProxyRepository(testContext.ApiClient);
        Movie movie = await movieRepository.GetMovieByIdAsync(ProxyRepoSeedIds.SeededMovieId)
            ?? throw new InvalidOperationException("Seeded movie was not found.");

        await movieRepository.AddOwnedMovieAsync(new OwnedMovie
        {
            User = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = movie.Id },
        });

        bool ownsMovie = await movieRepository.UserOwnsMovieAsync(ProxyRepoSeedIds.SeededUserId, movie.Id);

        Assert.True(ownsMovie);
    }
}
