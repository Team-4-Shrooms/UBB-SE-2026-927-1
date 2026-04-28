using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class EquipmentRepository : IEquipmentRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;

        public EquipmentRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        public List<Equipment> FetchAvailableEquipment()
        {
            return _context.Equipment
                .AsNoTracking()
                .Include(equipment => equipment.Seller)
                .Where(equipment => equipment.Status == EquipmentStatus.Available)
                .ToList();
        }

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            return await _context.Equipment
                .Include(e => e.Seller)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Equipment item)
        {
            await _context.Equipment.AddAsync(item);
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

