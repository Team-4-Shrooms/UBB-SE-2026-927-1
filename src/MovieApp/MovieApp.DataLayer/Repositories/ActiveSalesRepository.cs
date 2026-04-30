using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class ActiveSalesRepository : IActiveSalesRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;

        public ActiveSalesRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ActiveSale>> GetCurrentSalesAsync()
        {
            DateTime now = DateTime.UtcNow;

            return await _context.ActiveSales
                .AsNoTracking()
                .Include(sale => sale.Movie)
                .Where(sale => sale.StartTime <= now && sale.EndTime > now)
                .OrderBy(sale => sale.EndTime)
                .ToListAsync();
        }
    }
}

