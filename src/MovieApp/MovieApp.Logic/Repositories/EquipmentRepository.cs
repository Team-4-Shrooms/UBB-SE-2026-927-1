using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class EquipmentRepository : IEquipmentRepository
    {
        private readonly AppDbContext _context;

        public EquipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Equipment> FetchAvailableEquipment()
        {
            return _context.Equipment
                .AsNoTracking()
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
