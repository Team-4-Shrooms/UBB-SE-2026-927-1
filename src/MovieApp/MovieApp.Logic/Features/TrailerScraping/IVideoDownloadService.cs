using System.Threading.Tasks;

namespace MovieApp.Logic.Features.TrailerScraping
{
    /// <summary>
    /// Defines the contract for downloading videos from external sources.
    /// </summary>
    public interface IVideoDownloadService
    {
        string? LastError { get; }
        Task EnsureDependenciesAsync();
        Task<string?> DownloadVideoAsMp4Async(string youtubeUrl, int maxDurationSeconds = 60);
        string GetExpectedFilePath(string videoId);
    }
}
