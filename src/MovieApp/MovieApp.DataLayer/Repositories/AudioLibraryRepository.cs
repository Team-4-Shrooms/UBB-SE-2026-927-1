using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.DataLayer.Repositories
{
    /// <summary>
    /// EF Core data access for the MusicTrack table.
    /// </summary>
    public class AudioLibraryRepository : IAudioLibraryRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;
        private DbSet<MusicTrack> MusicTracks => _context.MusicTracks;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioLibraryRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public AudioLibraryRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IList<MusicTrack>> GetAllTracksAsync()
        {
            return await MusicTracks
                .OrderBy(track => track.TrackName)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<MusicTrack?> GetTrackByIdAsync(int musicTrackId)
        {
            return await MusicTracks.FindAsync(musicTrackId);
        }
    }
}
