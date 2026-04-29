using MovieApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Defines the contract for interacting with the audio library.
    /// </summary>
    public interface IAudioLibraryRepository
    {
        /// <summary>
        /// Retrieves all available music tracks.
        /// </summary>
        /// <returns>A list of all music tracks.</returns>
        Task<IList<MusicTrack>> GetAllTracksAsync();

        /// <summary>
        /// Retrieves a specific music track by its unique identifier.
        /// </summary>
        /// <param name="musicTrackId">The unique identifier of the music track to retrieve.</param>
        /// <returns>The music track if found, otherwise null.</returns>
        Task<MusicTrack?> GetTrackByIdAsync(int musicTrackId);
    }
}
