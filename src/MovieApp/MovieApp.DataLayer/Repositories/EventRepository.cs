using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class EventRepository : IEventRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;
        public EventRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }
        public async Task<List<MovieEvent>> GetEventsByMovieIdAsync(int movieId)
        {
            return await _context.MovieEvents
                .AsNoTracking()
                .Include(movieEvent => movieEvent.Movie)
                .Where(movieEvent => movieEvent.Movie.Id == movieId)
                .OrderBy(movieEvent => movieEvent.Date)
                .ThenBy(movieEvent => movieEvent.Id)
                .ToListAsync();
        }

        public async Task<List<MovieEvent>> GetAllEventsAsync()
        {
            return await _context.MovieEvents
                .AsNoTracking()
                .Include(movieEvent => movieEvent.Movie)
                .OrderBy(movieEvent => movieEvent.Date)
                .ThenBy(movieEvent => movieEvent.Id)
                .ToListAsync();
        }

        public async Task<MovieEvent?> GetEventByIdAsync(int eventId)
        {
            return await _context.MovieEvents
                .AsNoTracking()
                .Include(movieEvent => movieEvent.Movie)
                .FirstOrDefaultAsync(movieEvent => movieEvent.Id == eventId);
        }

        public async Task<bool> UserHasTicketAsync(int userId, int eventId)
        {
            return await _context.OwnedTickets
                .AnyAsync(ot => ot.User.Id == userId && ot.Event.Id == eventId);
        }

        public async Task AddOwnedTicketAsync(OwnedTicket ticket)
        {
            await _context.OwnedTickets.AddAsync(ticket);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

