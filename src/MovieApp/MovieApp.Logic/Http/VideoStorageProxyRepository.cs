using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs;

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
            var result = await _apiClient.PostAsync<Reel>("api/video-storage/reels", new InsertReelRequestBody
            {
                VideoUrl = reel.VideoUrl,
                ThumbnailUrl = reel.ThumbnailUrl,
                Title = reel.Title,
                Caption = reel.Caption,
                FeatureDurationSeconds = reel.FeatureDurationSeconds,
                CropDataJson = reel.CropDataJson,
                BackgroundMusicId = reel.BackgroundMusicId,
                Source = reel.Source,
                Genre = reel.Genre,
                CreatedAt = reel.CreatedAt,
                LastEditedAt = reel.LastEditedAt,
                MovieId = reel.Movie?.Id ?? 0,
                CreatorUserId = reel.CreatorUser?.Id ?? 0,
            });
            
            return result ?? reel;
        }
    }
}