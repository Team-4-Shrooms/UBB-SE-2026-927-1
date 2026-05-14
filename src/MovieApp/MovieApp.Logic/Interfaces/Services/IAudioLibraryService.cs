using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IAudioLibraryService
    {
        Task<List<MusicTrack>> GetAllTracksAsync();
        Task<MusicTrack?> GetTrackByIdAsync(int musicTrackId);
    }
}
