using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy
{
    public class MovieTournamentProxyRepository : IMovieTournamentRepository
    {
        private readonly ApiClient _apiClient;

        public MovieTournamentProxyRepository(ApiClient apiClient) => _apiClient = apiClient;

        public async Task<int> GetTournamentPoolSizeAsync(int userId)
        {
            return await _apiClient.GetAsync<int>($"api/movie-tournament/users/{userId}/pool-size");
        }

        public async Task<List<Movie>> GetTournamentPoolAsync(int userId, int poolSize)
        {
            var result = await _apiClient.GetAsync<List<Movie>>($"api/movie-tournament/users/{userId}/pool?poolSize={poolSize}");
            return result ?? new List<Movie>();
        }

        public async Task BoostMovieScoreAsync(int userId, int movieId, decimal scoreBoost)
        {
            await _apiClient.PostAsync($"api/movie-tournament/users/{userId}/movies/{movieId}/boost", new
            {
                ScoreBoost = scoreBoost,
            });
        }
    }
}
