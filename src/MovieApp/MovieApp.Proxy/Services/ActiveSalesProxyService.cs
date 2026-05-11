using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class ActiveSalesProxyService : IActiveSalesService
    {
        private readonly ApiClient _apiClient;

        public ActiveSalesProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Dictionary<int, decimal>> GetBestDiscountPercentByMovieIdAsync()
        {
            // The API only exposes GET /api/active-sales/current which returns all current sales.
            // Compute the best (highest) discount percentage per movie locally — mirrors ActiveSalesService logic.
            var sales = await _apiClient.GetAsync<List<ActiveSale>>("api/active-sales/current")
                        ?? new List<ActiveSale>();

            var bestByMovieId = new Dictionary<int, decimal>();
            foreach (ActiveSale sale in sales)
            {
                if (sale.Movie == null) continue;
                int movieId = sale.Movie.Id;
                decimal percentage = sale.DiscountPercentage;

                if (!bestByMovieId.TryGetValue(movieId, out decimal existing) || percentage > existing)
                    bestByMovieId[movieId] = percentage;
            }
            return bestByMovieId;
        }
    }
}
