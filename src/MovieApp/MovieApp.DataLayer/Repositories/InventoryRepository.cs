using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class InventoryRepository : IInventoryRepository
    {
        private const string RemoveOwnedMovieTransactionType = "RemoveOwnedMovie";
        private const string RemoveOwnedTicketTransactionType = "RemoveOwnedTicket";
        private const string CompletedTransactionStatus = "Completed";

        private readonly MovieApp.Logic.Data.IMovieAppDbContext _context;
        private DbSet<User> Users => _context.Users;
        private DbSet<Movie> Movies => _context.Movies;
        private DbSet<MovieEvent> MovieEvents => _context.MovieEvents;
        private DbSet<OwnedMovie> OwnedMovies => _context.OwnedMovies;
        private DbSet<OwnedTicket> OwnedTickets => _context.OwnedTickets;
        private DbSet<Transaction> Transactions => _context.Transactions;

        public InventoryRepository(MovieApp.Logic.Data.IMovieAppDbContext context)
        {
            _context = context;
        }

        public List<Movie> GetOwnedMovies(int userId)
        {
            return OwnedMovies
                .Where(ownedMovie => ownedMovie.User.Id == userId)
                .Select(ownedMovie => ownedMovie.Movie)
                .ToList();
        }

        public void RemoveOwnedMovie(int userId, int movieId)
        {
            if (userId <= 0)
            {
                return;
            }

            User? user = Users.FirstOrDefault(candidate => candidate.Id == userId);
            if (user is null)
            {
                return;
            }
            Movie? movie = Movies.FirstOrDefault(candidate => candidate.Id == movieId);
            List<OwnedMovie> ownerships = OwnedMovies
                .Where(ownedMovie => ownedMovie.User.Id == userId && ownedMovie.Movie.Id == movieId)
                .ToList();
            OwnedMovies.RemoveRange(ownerships);

            Transaction transaction = new Transaction
            {
                Buyer = user,
                Movie = movie,
                Amount = 0m,
                Type = RemoveOwnedMovieTransactionType,
                Status = CompletedTransactionStatus,
                Timestamp = DateTime.UtcNow
            };
            Transactions.Add(transaction);

            _context.SaveChanges();
        }

        public void RemoveOwnedTicket(int userId, int eventId)
        {
            if (userId <= 0)
            {
                return;
            }

            User? user = Users.FirstOrDefault(candidate => candidate.Id == userId);
            if (user is null)
            {
                return;
            }
            MovieEvent? movieEvent = MovieEvents.FirstOrDefault(candidate => candidate.Id == eventId);
            List<OwnedTicket> ownerships = OwnedTickets
                .Where(ownedTicket => ownedTicket.User.Id == userId && ownedTicket.Event.Id == eventId)
                .ToList();
            OwnedTickets.RemoveRange(ownerships);

            Transaction transaction = new Transaction
            {
                Buyer = user,
                Event = movieEvent,
                Amount = 0m,
                Type = RemoveOwnedTicketTransactionType,
                Status = CompletedTransactionStatus,
                Timestamp = DateTime.UtcNow
            };
            Transactions.Add(transaction);

            _context.SaveChanges();
        }

        public List<MovieEvent> GetOwnedTickets(int userId)
        {
            return OwnedTickets
                .Where(ownedTicket => ownedTicket.User.Id == userId)
                .Select(ownedTicket => ownedTicket.Event)
                .ToList();
        }

        public List<Equipment> GetOwnedEquipment(int userId)
        {
            return Transactions
                .Where(transaction => transaction.Buyer.Id == userId && transaction.Equipment != null)
                .Select(transaction => transaction.Equipment!)
                .ToList();
        }
    }
}
