using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ReelProxyServiceIntegrationTests
{
    [Fact]
    public async Task GetReelByIdAsync_SeededReel_ReturnsMatchingId()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        RecommendationProxyService recommendationRepository = new RecommendationProxyService(testContext.ApiClient);
        ReelProxyService reelRepository = new ReelProxyService(testContext.ApiClient);

        IList<Reel> allReels = await recommendationRepository.GetAllReelsAsync();
        int reelId = allReels[0].Id;
        Reel? reel = await reelRepository.GetReelByIdAsync(reelId);

        Assert.Equal(reelId, reel?.Id);
    }

    [Fact]
    public async Task UpdateReelEditsAsync_ExistingReel_UpdatesVideoUrl()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        RecommendationProxyService recommendationRepository = new RecommendationProxyService(testContext.ApiClient);
        ReelProxyService reelRepository = new ReelProxyService(testContext.ApiClient);

        IList<Reel> allReels = await recommendationRepository.GetAllReelsAsync();
        int reelId = allReels[0].Id;
        string updatedVideoUrl = $"https://example.com/reels/{Guid.NewGuid():N}.mp4";

        await reelRepository.UpdateReelEditsAsync(reelId, "{\"x\":1}", null, updatedVideoUrl);
        Reel? updatedReel = await reelRepository.GetReelByIdAsync(reelId);

        Assert.Equal(updatedVideoUrl, updatedReel?.VideoUrl);
    }
}




