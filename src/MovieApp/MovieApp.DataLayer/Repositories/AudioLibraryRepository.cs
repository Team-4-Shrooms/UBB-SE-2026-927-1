using Microsoft.EntityFrameworkCore;
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
        private readonly MovieApp.Logic.Data.IMovieAppDbContext _context;
        private DbSet<MusicTrack> MusicTracks => _context.MusicTracks;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioLibraryRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public AudioLibraryRepository(MovieApp.Logic.Data.IMovieAppDbContext context)
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
