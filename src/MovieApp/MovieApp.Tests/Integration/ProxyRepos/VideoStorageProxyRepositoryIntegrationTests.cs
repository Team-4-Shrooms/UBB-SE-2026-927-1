using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class VideoStorageProxyRepositoryIntegrationTests
{
    [Fact]
    public async Task InsertReelAsync_ValidReel_ReturnsPositiveIdentifier()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        VideoStorageProxyRepository videoStorageRepository = new VideoStorageProxyRepository(testContext.ApiClient);

        Reel insertedReel = await videoStorageRepository.InsertReelAsync(new Reel
        {
            VideoUrl = $"https://example.com/video-storage/{Guid.NewGuid():N}.mp4",
            ThumbnailUrl = "https://example.com/video-storage/thumbnail.png",
            Title = "Video Storage Reel",
            Caption = "Proxy repository integration test",
            FeatureDurationSeconds = 18.25m,
            CropDataJson = "{}",
            BackgroundMusicId = null,
            Source = "unit-test",
            Genre = "Action",
            CreatedAt = DateTime.UtcNow,
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
            CreatorUser = new User { Id = ProxyRepoSeedIds.SeededUserId },
        });

        Assert.True(insertedReel.Id > 0);
    }
}
