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
                .Where(ownedMovie => ownedMovie.User.Id == userId)
                .Select(ownedMovie => ownedMovie.Movie)
                .ToListAsync();
        }

        public async Task<List<OwnedMovie>> GetMovieOwnershipsAsync(int userId, int movieId)
        {
            return await _context.OwnedMovies
                .Include(ownedMovie => ownedMovie.User)
                .Include(ownedMovie => ownedMovie.Movie)
                .Where(ownedMovie => ownedMovie.User.Id == userId && ownedMovie.Movie.Id == movieId)
                .ToListAsync();
        }

        public Task RemoveMovieOwnershipsAsync(IEnumerable<OwnedMovie> ownerships)
        {
            _context.OwnedMovies.RemoveRange(ownerships);
            return Task.CompletedTask;
        }

        public async Task<List<OwnedTicket>> GetTicketOwnershipsAsync(int userId, int eventId)
        {
            return await _context.OwnedTickets
                .Include(ownedTicket => ownedTicket.Event)
                    .ThenInclude(movieEvent => movieEvent.Movie)
                .Where(ownedTicket => ownedTicket.User.Id == userId && ownedTicket.Event.Id == eventId)
                .ToListAsync();
        }

        public Task RemoveTicketOwnershipsAsync(IEnumerable<OwnedTicket> ownerships)
        {
            _context.OwnedTickets.RemoveRange(ownerships);
            return Task.CompletedTask;
        }

        public async Task<List<OwnedTicket>> GetAllTicketsForUserAsync(int userId)
        {
            return await _context.OwnedTickets
                .Include(ownedTicket => ownedTicket.Event)
                    .ThenInclude(movieEvent => movieEvent.Movie)
                .Where(ownedTicket => ownedTicket.User.Id == userId)
                .ToListAsync();
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

