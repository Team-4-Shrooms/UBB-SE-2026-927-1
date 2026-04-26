using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;
using System.Threading.Tasks;
using System;

namespace MovieApp.Logic.Repositories
{
    /// <summary>
    /// EF Core data access for inserting Reel records into the database.
    /// </summary>
    public class VideoStorageRepository : IVideoStorageRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoStorageRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public VideoStorageRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Reel> InsertReelAsync(Reel reel)
        {
            reel.CreatedAt = DateTime.UtcNow;

            _context.Reels.Add(reel);
            await _context.SaveChangesAsync();

            return reel;
        }
    }
}
