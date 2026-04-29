using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace MovieApp.DataLayer.Repositories
{
    /// <summary>
    /// EF Core data access for inserting Reel records into the database.
    /// </summary>
    public class VideoStorageRepository : IVideoStorageRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;
        private DbSet<Reel> Reels => _context.Reels;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoStorageRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public VideoStorageRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Reel> InsertReelAsync(Reel reel)
        {
            reel.CreatedAt = DateTime.UtcNow;

            Reels.Add(reel);
            await _context.SaveChangesAsync();

            return reel;
        }
    }
}
