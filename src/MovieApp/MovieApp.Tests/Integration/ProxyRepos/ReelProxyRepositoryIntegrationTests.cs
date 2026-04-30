using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ReelProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task GetReelByIdAsync_SeededReel_ReturnsMatchingId()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        RecommendationProxyRepository recommendationRepository = new RecommendationProxyRepository(testContext.ApiClient);
        ReelProxyRepository reelRepository = new ReelProxyRepository(testContext.ApiClient);

        IList<Reel> allReels = await recommendationRepository.GetAllReelsAsync();
        int reelId = allReels[0].Id;
        Reel? reel = await reelRepository.GetReelByIdAsync(reelId);

        Assert.Equal(reelId, reel?.Id);
    }

    [Fact]
    public async Task UpdateReelEditsAsync_ExistingReel_UpdatesVideoUrl()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        RecommendationProxyRepository recommendationRepository = new RecommendationProxyRepository(testContext.ApiClient);
        ReelProxyRepository reelRepository = new ReelProxyRepository(testContext.ApiClient);

        IList<Reel> allReels = await recommendationRepository.GetAllReelsAsync();
        int reelId = allReels[0].Id;
        string updatedVideoUrl = $"https://example.com/reels/{Guid.NewGuid():N}.mp4";

        await reelRepository.UpdateReelEditsAsync(reelId, "{\"x\":1}", null, updatedVideoUrl);
        Reel? updatedReel = await reelRepository.GetReelByIdAsync(reelId);

        Assert.Equal(updatedVideoUrl, updatedReel?.VideoUrl);
    }
}
