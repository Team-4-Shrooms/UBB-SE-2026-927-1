using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ScrapeJobProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task CreateJobAsync_NewJob_ReturnsPositiveIdentifier()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        ScrapeJobProxyRepository scrapeJobRepository = new ScrapeJobProxyRepository(testContext.ApiClient);

        int createdJobId = await scrapeJobRepository.CreateJobAsync(new ScrapeJob
        {
            SearchQuery = "Inception",
            MaxResults = 5,
            Status = "running",
            MoviesFound = 0,
            ReelsCreated = 0,
            StartedAt = DateTime.UtcNow,
        });

        Assert.True(createdJobId > 0);
    }

    [Fact]
    public async Task InsertScrapedReelAsync_ValidReel_ReturnsPositiveIdentifier()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        ScrapeJobProxyRepository scrapeJobRepository = new ScrapeJobProxyRepository(testContext.ApiClient);

        int createdReelId = await scrapeJobRepository.InsertScrapedReelAsync(new Reel
        {
            VideoUrl = $"https://example.com/scraped/{Guid.NewGuid():N}.mp4",
            ThumbnailUrl = "https://example.com/scraped/thumbnail.png",
            Title = "Scraped Reel",
            Caption = "Proxy repository integration test",
            FeatureDurationSeconds = 12.5m,
            CropDataJson = "{}",
            Source = "unit-test",
            Genre = "Drama",
            CreatedAt = DateTime.UtcNow,
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
            CreatorUser = new User { Id = ProxyRepoSeedIds.SeededUserId },
        });

        Assert.True(createdReelId > 0);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_SeededDatabase_ReturnsPositiveMovieCount()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        ScrapeJobProxyRepository scrapeJobRepository = new ScrapeJobProxyRepository(testContext.ApiClient);

        DashboardStatsModel stats = await scrapeJobRepository.GetDashboardStatsAsync();

        Assert.True(stats.TotalMovies > 0);
    }
}
