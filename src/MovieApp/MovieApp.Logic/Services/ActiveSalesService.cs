using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Services
{
    public class ActiveSalesService  : IActiveSalesService
    {
        private readonly IActiveSalesRepository _repository;
        public ActiveSalesService(IActiveSalesRepository repository) => _repository = repository;
        public async Task<Dictionary<int, decimal>> GetBestDiscountPercentByMovieIdAsync()
        {
            List<ActiveSale> sales = await _repository.GetCurrentSalesAsync();

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

