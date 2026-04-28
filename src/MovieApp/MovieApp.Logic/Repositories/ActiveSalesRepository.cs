using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class ActiveSalesRepository : IActiveSalesRepository
    {
        private readonly AppDbContext _context;

        public ActiveSalesRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<ActiveSale> GetCurrentSales()
        {
            DateTime now = DateTime.UtcNow;

            return _context.ActiveSales
                .AsNoTracking()
                .Include(sale => sale.Movie)
                .Where(sale => sale.StartTime <= now && sale.EndTime > now)
                .OrderBy(sale => sale.EndTime)
                .ToList();
        }
    }
}
