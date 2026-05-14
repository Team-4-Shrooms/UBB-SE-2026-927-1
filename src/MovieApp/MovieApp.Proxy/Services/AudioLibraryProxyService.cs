using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class AudioLibraryProxyService : IAudioLibraryService
    {
        private readonly ApiClient _apiClient;

        public AudioLibraryProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<MusicTrack>> GetAllTracksAsync()
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
