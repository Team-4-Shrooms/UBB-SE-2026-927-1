using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Http
{
    public class MovieTournamentProxyRepository : IMovieTournamentRepository
    {
        private readonly ApiClient _apiClient;

        public MovieTournamentProxyRepository(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<int> GetTournamentPoolSizeAsync(int userId)
        {
            var results = await _apiClient.GetAllAsync<int>($"api/movie-tournament/users/{userId}/pool-size");
            return results.Count > 0 ? results[0] : 0;
        }

        public async Task<List<Movie>> GetTournamentPoolAsync(int userId, int poolSize)
        {
            return await _apiClient.GetAllAsync<Movie>($"api/movie-tournament/users/{userId}/pool?poolSize={poolSize}");
        }

        public async Task BoostMovieScoreAsync(int userId, int movieId, decimal scoreBoost)
        {
            await _apiClient.PostAsync($"api/movie-tournament/users/{userId}/movies/{movieId}/boost", new { scoreBoost });
        }
    }
}