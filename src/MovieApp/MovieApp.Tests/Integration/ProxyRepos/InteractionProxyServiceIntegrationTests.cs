using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ReelInteractionProxyServiceIntegrationTests
{
    [Fact]
    public async Task InsertInteractionAsync_ExistingUserAndReel_CreatesInteractionRecord()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        MovieProxyService movieRepository = new MovieProxyService(testContext.ApiClient);
        ScrapeJobProxyService scrapeJobRepository = new ScrapeJobProxyService(testContext.ApiClient);
        ReelInteractionProxyService interactionRepository = new ReelInteractionProxyService(testContext.ApiClient);

        Movie movie = await movieRepository.GetMovieByIdAsync(ProxyRepoSeedIds.SeededMovieId)
            ?? throw new InvalidOperationException("Seeded movie was not found.");
        int createdReelId = await scrapeJobRepository.InsertScrapedReelAsync(new Reel
        {
            VideoUrl = $"https://example.com/interactions/{Guid.NewGuid():N}.mp4",
            ThumbnailUrl = "https://example.com/interactions/thumbnail.png",
            Title = "Interaction Reel",
            Caption = "Proxy repository integration test",
            FeatureDurationSeconds = 11m,
            Source = "unit-test",
            Genre = "Drama",
            CreatedAt = DateTime.UtcNow,
            Movie = new Movie { Id = movie.Id },
            CreatorUser = new User { Id = ProxyRepoSeedIds.SeededUserId },
        });

        await interactionRepository.InsertInteractionAsync(new UserReelInteraction
        {
            IsLiked = true,
            WatchDurationSeconds = 12.5m,
            WatchPercentage = 80m,
            ViewedAt = DateTime.UtcNow,
            User = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Reel = new Reel { Id = createdReelId },
        });

        int likeCount = await interactionRepository.GetLikeCountAsync(createdReelId);

        Assert.Equal(1, likeCount);
    }
}




