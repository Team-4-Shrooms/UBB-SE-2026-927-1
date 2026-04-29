using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
{
    public class AudioLibraryProxyRepository : IAudioLibraryRepository
    {
        private readonly ApiClient _apiClient;

        public AudioLibraryProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IList<MusicTrack>> GetAllTracksAsync()
        {
            return await _apiClient.GetAllAsync<MusicTrack>("api/audio-library/tracks");
        }

        public async Task<MusicTrack?> GetTrackByIdAsync(int musicTrackId)
        {
            return await _apiClient.GetByIdAsync<MusicTrack>("api/audio-library/tracks", musicTrackId);
        }
    }
}