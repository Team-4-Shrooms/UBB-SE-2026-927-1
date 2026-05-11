using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class ActiveSalesProxyRepository : IActiveSalesRepository
    {
        private readonly ApiClient _apiClient;

        public ActiveSalesProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<List<ActiveSale>> GetCurrentSalesAsync()
        {
            var result = await _apiClient.GetAsync<List<ActiveSale>>("api/active-sales/current");
            return result ?? new List<ActiveSale>();
        }
    }
}
