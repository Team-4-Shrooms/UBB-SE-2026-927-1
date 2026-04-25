using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    public interface IActiveSalesRepository
    {
        Dictionary<int, decimal> GetBestDiscountPercentByMovieId();

        List<ActiveSale> GetCurrentSales();
    }
}
