using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class MovieRepository : IMovieRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;
        public MovieRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<Movie?> GetMovieByIdAsync(int movieId)
        {
            return await _context.Movies.FindAsync(movieId);
        }

        public async Task<bool> UserOwnsMovieAsync(int userId, int movieId)
        {
            return await _context.OwnedMovies.AnyAsync(om => om.User.Id == userId && om.Movie.Id == movieId);
        }

        public async Task AddOwnedMovieAsync(OwnedMovie ownership)
        {
            await _context.OwnedMovies.AddAsync(ownership);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Movie>> SearchMoviesAsync(string pattern, int limit)
        {
            return await _context.Movies.Where(m => m.Title.Contains(pattern)).Take(limit).ToListAsync();
        }
    }
}

