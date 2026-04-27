using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.Logic.Repositories
{
    /// <summary>
    /// EF Core data access for the MusicTrack table.
    /// </summary>
    public class AudioLibraryRepository : IAudioLibraryRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioLibraryRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public AudioLibraryRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IList<MusicTrack>> GetAllTracksAsync()
        {
            return await _context.MusicTracks
                .OrderBy(track => track.TrackName)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<MusicTrack?> GetTrackByIdAsync(int musicTrackId)
        {
            return await _context.MusicTracks.FindAsync(musicTrackId);
        }
    }
}
