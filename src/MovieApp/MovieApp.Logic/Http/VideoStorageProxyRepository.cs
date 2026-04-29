using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Http
{
    public class VideoStorageProxyRepository : IVideoStorageRepository
    {
        private readonly ApiClient _apiClient;

        public VideoStorageProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Reel> InsertReelAsync(Reel reel)
        {
            var result = await _apiClient.PostAsync<Reel>("api/video-storage/reels", new
            {
                videoUrl = reel.VideoUrl,
                thumbnailUrl = reel.ThumbnailUrl,
                title = reel.Title,
                caption = reel.Caption,
                featureDurationSeconds = reel.FeatureDurationSeconds,
                cropDataJson = reel.CropDataJson,
                backgroundMusicId = reel.BackgroundMusicId,
                source = reel.Source,
                genre = reel.Genre,
                createdAt = reel.CreatedAt,
                lastEditedAt = reel.LastEditedAt,
                movieId = reel.Movie?.Id ?? 0,
                creatorUserId = reel.CreatorUser?.Id ?? 0,
            });
            
            return result ?? reel;
        }
    }
}