using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class InventoryRepository : IInventoryRepository
    {
        private readonly Interfaces.IMovieAppDbContext _context;

        public InventoryRepository(Interfaces.IMovieAppDbContext context)
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

        public async Task<List<Equipment>> GetOwnedEquipmentAsync(int userId)
        {
            List<int> ownedIds = await _context.Transactions
                .Include(t => t.Equipment)
                .Where(t => t.Buyer.Id == userId && t.Type == "EquipmentPurchase" && t.Equipment != null)
                .Select(t => t.Equipment!.Id)
                .ToListAsync();

            return await _context.Equipment
                .Where(e => ownedIds.Contains(e.Id))
                .ToListAsync();
        }

        public async Task<Equipment?> GetEquipmentByIdAsync(int equipmentId)
        {
            return await _context.Equipment.FindAsync(equipmentId);
        }

        public async Task RemoveOwnedEquipmentAsync(int userId, int equipmentId)
        {
            Transaction? transaction = await _context.Transactions
                .Include(t => t.Equipment)
                .FirstOrDefaultAsync(t => t.Buyer.Id == userId && t.Equipment!.Id == equipmentId && t.Type == "EquipmentPurchase");

            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            Equipment? equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment != null)
            {
                equipment.Status = EquipmentStatus.Available;
            }
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
