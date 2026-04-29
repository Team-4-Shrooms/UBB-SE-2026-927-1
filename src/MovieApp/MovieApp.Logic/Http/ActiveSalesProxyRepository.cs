using System.Collections.Generic;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Http
{
    public class ActiveSalesProxyRepository : IActiveSalesRepository
    {
        private readonly ApiClient _apiClient;

        public ActiveSalesProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public List<ActiveSale> GetCurrentSales()
        {
            return _apiClient.GetAllAsync<ActiveSale>("api/active-sales/current").GetAwaiter().GetResult();
        }
    }
}
