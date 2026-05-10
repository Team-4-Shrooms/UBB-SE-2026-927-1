using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.MovieSwipe;

namespace MovieApp.Proxy.Services
{
    public class MovieCardFeedProxyService : IMovieCardFeedService
    {
        private readonly ApiClient _apiClient;

        public MovieCardFeedProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Movie>> FetchMovieFeedAsync(int userId, int count)
        {
            var result = await _apiClient.GetAsync<List<Movie>>($"api/moviecardfeed/{userId}?count={count}");
            return result ?? new List<Movie>();
        }
    }
}
