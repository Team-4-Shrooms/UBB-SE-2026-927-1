using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class AudioLibraryProxyRepository : IAudioLibraryRepository
    {
        private readonly ApiClient _apiClient;

        public AudioLibraryProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<IList<MusicTrack>> GetAllTracksAsync()
        {
            var result = await _apiClient.GetAsync<List<MusicTrack>>("api/audio-library/tracks");
            return result ?? new List<MusicTrack>();
        }

        public async Task<MusicTrack?> GetTrackByIdAsync(int musicTrackId)
        {
            return await _apiClient.GetAsync<MusicTrack>($"api/audio-library/tracks/{musicTrackId}");
        }
    }
}
