using System.Collections.Generic;
using System.Threading.Tasks;
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
            return await _apiClient.GetAsync<Dictionary<int, decimal>>("api/active-sales/current")
                   ?? new Dictionary<int, decimal>();
        }
    }
}
