using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class InventoryRepository : IInventoryRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;
        public InventoryRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetOwnedMoviesAsync(int userId)
        {
            return await _context.OwnedMovies
                .Include(ownedMovie => ownedMovie.Movie)
                .Where(om => om.User.Id == userId)
                .Select(om => om.Movie)
                .ToListAsync();
        }

        public async Task<List<OwnedMovie>> GetMovieOwnershipsAsync(int userId, int movieId)
        {
            return await _context.OwnedMovies
                .Include(om => om.User)
                .Include(om => om.Movie)
                .Where(om => om.User.Id == userId && om.Movie.Id == movieId)
                .ToListAsync();
        }

        public void RemoveMovieOwnerships(IEnumerable<OwnedMovie> ownerships)
        {
            _context.OwnedMovies.RemoveRange(ownerships);
        }

        public async Task<List<OwnedTicket>> GetTicketOwnershipsAsync(int userId, int eventId)
        {
            return await _context.OwnedTickets
                .Include(ownedTicket => ownedTicket.Event)
                    .ThenInclude(movieEvent => movieEvent.Movie)
                .Where(ot => ot.User.Id == userId && ot.Event.Id == eventId)
                .ToListAsync();
        }

        public void RemoveTicketOwnerships(IEnumerable<OwnedTicket> ownerships)
        {
            _context.OwnedTickets.RemoveRange(ownerships);
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

