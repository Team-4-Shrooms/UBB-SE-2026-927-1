using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
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
            var result = await _apiClient.PostAsync<Reel>("api/video-storage/reels", reel);
            
            return result ?? reel;
        }
    }
}