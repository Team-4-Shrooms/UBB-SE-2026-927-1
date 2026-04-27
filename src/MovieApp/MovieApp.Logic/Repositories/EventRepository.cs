using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class EventRepository : IEventRepository
    {
        private const string EventTicketTransactionType = "EventTicket";
        private const string CompletedTransactionStatus = "Completed";

        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<MovieEvent> GetEventsForMovie(int movieId)
        {
            return _context.MovieEvents
                .AsNoTracking()
                .Where(movieEvent => movieEvent.Movie.Id == movieId)
                .OrderBy(movieEvent => movieEvent.Date)
                .ThenBy(movieEvent => movieEvent.Id)
                .ToList();
        }

        public List<MovieEvent> GetAllEvents()
        {
            return _context.MovieEvents
                .AsNoTracking()
                .OrderBy(movieEvent => movieEvent.Date)
                .ThenBy(movieEvent => movieEvent.Id)
                .ToList();
        }

        public MovieEvent? GetEventById(int eventId)
        {
            return _context.MovieEvents
                .AsNoTracking()
                .FirstOrDefault(movieEvent => movieEvent.Id == eventId);
        }

        public void PurchaseTicket(int userId, int eventId)
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

            MovieEvent? movieEvent = _context.MovieEvents
                .FirstOrDefault(candidate => candidate.Id == eventId);
            if (movieEvent is null)
            {
                throw new InvalidOperationException("Event not found.");
            }

            bool alreadyOwned = _context.OwnedTickets
                .Any(ownedTicket => ownedTicket.User.Id == userId && ownedTicket.Event.Id == eventId);
            if (alreadyOwned)
            {
                throw new InvalidOperationException("You already own a ticket for this event.");
            }

            decimal price = movieEvent.TicketPrice;
            if (user.Balance < price)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            user.Balance -= price;

            OwnedTicket ticket = new OwnedTicket
            {
                User = user,
                Event = movieEvent,
                PurchaseDate = DateTime.UtcNow
            };
            _context.OwnedTickets.Add(ticket);

            Transaction transaction = new Transaction
            {
                Buyer = user,
                Event = movieEvent,
                Amount = -price,
                Type = EventTicketTransactionType,
                Status = CompletedTransactionStatus,
                Timestamp = DateTime.UtcNow
            };
            _context.Transactions.Add(transaction);

            _context.SaveChanges();
        }
    }
}
