using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;
        public EventRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<MovieEvent>> GetEventsByMovieIdAsync(int movieId)
        {
            return await _context.MovieEvents
                .AsNoTracking()
                .Where(e => e.Movie.Id == movieId)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<List<MovieEvent>> GetAllEventsAsync()
        {
            return await _context.MovieEvents
                .AsNoTracking()
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<MovieEvent?> GetEventByIdAsync(int eventId)
        {
            return await _context.MovieEvents.FindAsync(eventId);
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
