using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class MovieRepository : IMovieRepository
    {
        private const string MoviePurchaseTransactionType = "MoviePurchase";
        private const string CompletedTransactionStatus = "Completed";
        private const int MaxSearchResults = 10;

        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;
        private DbSet<Movie> Movies => _context.Movies;
        private DbSet<User> Users => _context.Users;
        private DbSet<OwnedMovie> OwnedMovies => _context.OwnedMovies;
        private DbSet<Transaction> Transactions => _context.Transactions;

        public MovieRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        public List<Movie> GetAllMovies()
        {
            return Movies
                .AsNoTracking()
                .OrderBy(movie => movie.Title)
                .ToList();
        }

        public Movie? GetMovieById(int movieId)
        {
            return Movies
                .AsNoTracking()
                .FirstOrDefault(movie => movie.Id == movieId);
        }

        public bool UserOwnsMovie(int userId, int movieId)
        {
            if (userId <= 0)
            {
                return false;
            }

            return OwnedMovies
                .Any(ownedMovie => ownedMovie.User.Id == userId && ownedMovie.Movie.Id == movieId);
        }

        public void PurchaseMovie(int userId, int movieId, decimal finalPrice)
        {
            if (userId <= 0)
            {
                throw new InvalidOperationException("You must be logged in to purchase.");
            }

            User? user = Users.FirstOrDefault(candidate => candidate.Id == userId);
            if (user is null)
            {
                throw new InvalidOperationException("User not found.");
            }
            Movie? movie = Movies.FirstOrDefault(candidate => candidate.Id == movieId);
            if (movie is null)
            {
                throw new InvalidOperationException("Movie not found.");
            }
            bool alreadyOwned = OwnedMovies
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
            OwnedMovies.Add(ownership);

            Transaction transaction = new Transaction
            {
                Buyer = user,
                Movie = movie,
                Amount = -finalPrice,
                Type = MoviePurchaseTransactionType,
                Status = CompletedTransactionStatus,
                Timestamp = DateTime.UtcNow
            };
            Transactions.Add(transaction);

            _context.SaveChanges();
        }

        public async Task<List<Movie>> SearchTop10MoviesAsync(string partialMovieName)
        {
            string pattern = partialMovieName ?? string.Empty;

            return await Movies
                .AsNoTracking()
                .Where(movie => movie.Title.Contains(pattern))
                .OrderBy(movie => movie.Title)
                .Take(MaxSearchResults)
                .ToListAsync();
        }
    }
}
