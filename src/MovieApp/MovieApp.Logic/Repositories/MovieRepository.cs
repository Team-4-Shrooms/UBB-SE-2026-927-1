using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class MovieRepository : IMovieRepository
    {
        private readonly AppDbContext _context;
        public MovieRepository(AppDbContext context)
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
