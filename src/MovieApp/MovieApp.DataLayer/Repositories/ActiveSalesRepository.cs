using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class ActiveSalesRepository : IActiveSalesRepository
    {
        private readonly MovieApp.Logic.Data.IMovieAppDbContext _context;
        private DbSet<ActiveSale> ActiveSales => _context.ActiveSales;

        public ActiveSalesRepository(MovieApp.Logic.Data.IMovieAppDbContext context)
        {
            _context = context;
        }

        public Dictionary<int, decimal> GetBestDiscountPercentByMovieId()
        {
            List<ActiveSale> sales = GetCurrentSales();

            Dictionary<int, decimal> bestByMovieId = new Dictionary<int, decimal>();
            foreach (ActiveSale sale in sales)
            {
                int movieId = sale.Movie.Id;
                decimal percentage = sale.DiscountPercentage;

                if (!bestByMovieId.TryGetValue(movieId, out decimal existing) || percentage > existing)
                {
                    bestByMovieId[movieId] = percentage;
                }
            }

            return bestByMovieId;
        }

        public List<ActiveSale> GetCurrentSales()
        {
            DateTime now = DateTime.UtcNow;

            return ActiveSales
                .AsNoTracking()
                .Include(sale => sale.Movie)
                .Where(sale => sale.StartTime <= now && sale.EndTime > now)
                .OrderBy(sale => sale.EndTime)
                .ToList();
        }
    }
}
