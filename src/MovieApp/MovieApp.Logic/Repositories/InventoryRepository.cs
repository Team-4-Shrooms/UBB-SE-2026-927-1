using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class InventoryRepository : IInventoryRepository
    {
        private const string RemoveOwnedMovieTransactionType = "RemoveOwnedMovie";
        private const string RemoveOwnedTicketTransactionType = "RemoveOwnedTicket";
        private const string CompletedTransactionStatus = "Completed";

        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Movie> GetOwnedMovies(int userId)
        {
            return _context.OwnedMovies
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

            User? user = _context.Users.FirstOrDefault(candidate => candidate.Id == userId);
            if (user is null)
            {
                return;
            }

            Movie? movie = _context.Movies.FirstOrDefault(candidate => candidate.Id == movieId);
            List<OwnedMovie> ownerships = _context.OwnedMovies
                .Where(ownedMovie => ownedMovie.User.Id == userId && ownedMovie.Movie.Id == movieId)
                .ToList();
            _context.OwnedMovies.RemoveRange(ownerships);

            Transaction transaction = new Transaction
            {
                Buyer = user,
                Movie = movie,
                Amount = 0m,
                Type = RemoveOwnedMovieTransactionType,
                Status = CompletedTransactionStatus,
                Timestamp = DateTime.UtcNow
            };
            _context.Transactions.Add(transaction);

            _context.SaveChanges();
        }

        public void RemoveOwnedTicket(int userId, int eventId)
        {
            if (userId <= 0)
            {
                return;
            }

            User? user = _context.Users.FirstOrDefault(candidate => candidate.Id == userId);
            if (user is null)
            {
                return;
            }

            MovieEvent? movieEvent = _context.MovieEvents.FirstOrDefault(candidate => candidate.Id == eventId);
            List<OwnedTicket> ownerships = _context.OwnedTickets
                .Where(ownedTicket => ownedTicket.User.Id == userId && ownedTicket.Event.Id == eventId)
                .ToList();
            _context.OwnedTickets.RemoveRange(ownerships);

            Transaction transaction = new Transaction
            {
                Buyer = user,
                Event = movieEvent,
                Amount = 0m,
                Type = RemoveOwnedTicketTransactionType,
                Status = CompletedTransactionStatus,
                Timestamp = DateTime.UtcNow
            };
            _context.Transactions.Add(transaction);

            _context.SaveChanges();
        }

        public List<MovieEvent> GetOwnedTickets(int userId)
        {
            return _context.OwnedTickets
                .Where(ownedTicket => ownedTicket.User.Id == userId)
                .Select(ownedTicket => ownedTicket.Event)
                .ToList();
        }

        public List<Equipment> GetOwnedEquipment(int userId)
        {
            return _context.Transactions
                .Where(transaction => transaction.Buyer.Id == userId && transaction.Equipment != null)
                .Select(transaction => transaction.Equipment!)
                .ToList();
        }
    }
}
