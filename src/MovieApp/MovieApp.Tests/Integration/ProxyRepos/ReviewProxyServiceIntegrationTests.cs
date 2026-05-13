using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ReviewProxyServiceIntegrationTests
{
    [Fact]
    public async Task AddReviewAsync_ExistingUserAndMovie_MakesReviewVisibleForMovie()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        MovieProxyService movieRepository = new MovieProxyService(testContext.ApiClient);
        ReviewProxyService reviewRepository = new ReviewProxyService(testContext.ApiClient);
        Movie movie = await movieRepository.GetMovieByIdAsync(ProxyRepoSeedIds.SeededMovieId)
            ?? throw new InvalidOperationException("Seeded movie was not found.");
        string uniqueComment = $"Proxy review {Guid.NewGuid():N}";

        await reviewRepository.PostReviewAsync(movie.Id, ProxyRepoSeedIds.SeededUserId, 8, uniqueComment);

        List<MovieReview> reviews = await reviewRepository.GetReviewsForMovieAsync(movie.Id);

        Assert.True(reviews.Any(review => review.Comment == uniqueComment));
    }
}

