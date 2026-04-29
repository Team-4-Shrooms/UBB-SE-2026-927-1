using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Interfaces.Services;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Services
{
    public class ActiveSalesService  : IActiveSalesService
    {
        private readonly IActiveSalesRepository _repository;
        public ActiveSalesService(IActiveSalesRepository repository) => _repository = repository;
        public Dictionary<int, decimal> GetBestDiscountPercentByMovieId()
        {
            List<ActiveSale> sales = _repository.GetCurrentSales();

            Dictionary<int, decimal> bestByMovieId = new Dictionary<int, decimal>();
            foreach (ActiveSale sale in sales)
            {
                if (sale.Movie == null) continue;
                int movieId = sale.Movie.Id;
                decimal percentage = sale.DiscountPercentage;

                if (!bestByMovieId.TryGetValue(movieId, out decimal existing) || percentage > existing)
                {
                    bestByMovieId[movieId] = percentage;
                }
            }

            return bestByMovieId;
        }
    }
}

