using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public class AudioLibraryService : IAudioLibraryService
    {
        private readonly IAudioLibraryRepository _audioLibraryRepository;

        public AudioLibraryService(IAudioLibraryRepository audioLibraryRepository)
        {
            _audioLibraryRepository = audioLibraryRepository;
        }

        public async Task<List<MusicTrack>> GetAllTracksAsync()
        {
            var tracks = await _audioLibraryRepository.GetAllTracksAsync();
            return tracks.ToList();
        }

        public async Task<MusicTrack?> GetTrackByIdAsync(int musicTrackId)
        {
            return await _audioLibraryRepository.GetTrackByIdAsync(musicTrackId);
        }
    }
}
