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

            // Cast the interface back to DbContext so we can access the tracker
            var dbContext = (DbContext)_context;

            // Swap out the "fake" movie stub for the real tracked movie
            if (reel.Movie != null)
            {
                var trackedMovie = await dbContext.Set<Movie>().FindAsync(reel.Movie.Id);
                reel.Movie = trackedMovie;
            }

            // Swap out the "fake" user stub for the real tracked user
            if (reel.CreatorUser != null)
            {
                var trackedUser = await dbContext.Set<User>().FindAsync(reel.CreatorUser.Id);
                reel.CreatorUser = trackedUser;
            }

            Reels.Add(reel);
            await _context.SaveChangesAsync();

            return reel;
        }
    }
}
