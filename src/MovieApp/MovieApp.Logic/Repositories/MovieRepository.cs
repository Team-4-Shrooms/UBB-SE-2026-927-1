using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class MovieRepository : IMovieRepository
    {
        private const string MoviePurchaseTransactionType = "MoviePurchase";
        private const string CompletedTransactionStatus = "Completed";

        private readonly AppDbContext _context;

        public MovieRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Movie> GetAllMovies()
        {
            return _context.Movies
                .AsNoTracking()
                .OrderBy(movie => movie.Title)
                .ToList();
        }

        public Movie? GetMovieById(int movieId)
        {
            return _context.Movies
                .AsNoTracking()
                .FirstOrDefault(movie => movie.Id == movieId);
        }

        public bool UserOwnsMovie(int userId, int movieId)
        {
            if (userId <= 0)
            {
                return false;
            }

            return _context.OwnedMovies
                .Any(ownedMovie => ownedMovie.User.Id == userId && ownedMovie.Movie.Id == movieId);
        }

        public void PurchaseMovie(int userId, int movieId, decimal finalPrice)
        {
            if (userId <= 0)
            {
                throw new InvalidOperationException("You must be logged in to purchase.");
            }

            User? user = _context.Users.FirstOrDefault(candidate => candidate.Id == userId);
            if (user is null)
            {
                throw new InvalidOperationException("User not found.");
            }

            Movie? movie = _context.Movies.FirstOrDefault(candidate => candidate.Id == movieId);
            if (movie is null)
            {
                throw new InvalidOperationException("Movie not found.");
            }

            bool alreadyOwned = _context.OwnedMovies
                .Any(ownedMovie => ownedMovie.User.Id == userId && ownedMovie.Movie.Id == movieId);
            if (alreadyOwned)
            {
                throw new InvalidOperationException("You already own this movie.");
            }

            if (user.Balance < finalPrice)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            user.Balance -= finalPrice;

            OwnedMovie ownership = new OwnedMovie
            {
                User = user,
                Movie = movie,
                PurchaseDate = DateTime.UtcNow
            };
            _context.OwnedMovies.Add(ownership);

            Transaction transaction = new Transaction
            {
                Buyer = user,
                Movie = movie,
                Amount = -finalPrice,
                Type = MoviePurchaseTransactionType,
                Status = CompletedTransactionStatus,
                Timestamp = DateTime.UtcNow
            };
            _context.Transactions.Add(transaction);

            _context.SaveChanges();
        }

        public async Task<List<Movie>> SearchTop10MoviesAsync(string partialMovieName)
        {
            string pattern = partialMovieName ?? string.Empty;

            return await _context.Movies
                .AsNoTracking()
                .Where(movie => movie.Title.Contains(pattern))
                .OrderBy(movie => movie.Title)
                .Take(10)
                .ToListAsync();
        }
    }
}
