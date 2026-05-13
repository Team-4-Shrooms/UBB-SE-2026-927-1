using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class AudioLibraryProxyServiceIntegrationTests
{
    [Fact]
    public async Task GetAllTracksAsync_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        AudioLibraryProxyService audioLibraryRepository = new AudioLibraryProxyService(testContext.ApiClient);

        IList<MusicTrack> allTracks = await audioLibraryRepository.GetAllTracksAsync();

        Assert.NotEmpty(allTracks);
    }

    [Fact]
    public async Task GetTrackByIdAsync_FirstTrack_ReturnsMatchingId()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        AudioLibraryProxyService audioLibraryRepository = new AudioLibraryProxyService(testContext.ApiClient);

        IList<MusicTrack> allTracks = await audioLibraryRepository.GetAllTracksAsync();
        int firstTrackId = allTracks[0].Id;
        MusicTrack? track = await audioLibraryRepository.GetTrackByIdAsync(firstTrackId);

        Assert.Equal(firstTrackId, track?.Id);
    }
}




