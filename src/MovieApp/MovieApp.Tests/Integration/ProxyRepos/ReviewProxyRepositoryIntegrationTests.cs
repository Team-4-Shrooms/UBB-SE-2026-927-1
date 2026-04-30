using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ReviewProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task AddReviewAsync_ExistingUserAndMovie_MakesReviewVisibleForMovie()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        MovieProxyRepository movieRepository = new MovieProxyRepository(testContext.ApiClient);
        ReviewProxyRepository reviewRepository = new ReviewProxyRepository(testContext.ApiClient);
        Movie movie = await movieRepository.GetMovieByIdAsync(ProxyRepoSeedIds.SeededMovieId)
            ?? throw new InvalidOperationException("Seeded movie was not found.");
        string uniqueComment = $"Proxy review {Guid.NewGuid():N}";

        await reviewRepository.AddReviewAsync(new MovieReview
        {
            StarRating = 8m,
            Comment = uniqueComment,
            CreatedAt = DateTime.UtcNow,
            User = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = movie.Id },
        });

        List<MovieReview> reviews = await reviewRepository.GetReviewsForMovieAsync(movie.Id);

        Assert.True(reviews.Any(review => review.Comment == uniqueComment));
    }
}
