using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public class MovieTournamentService : IMovieTournamentService
    {
        private readonly IMovieTournamentRepository _movieTournamentRepository;

        public MovieTournamentService(IMovieTournamentRepository movieTournamentRepository)
        {
            _movieTournamentRepository = movieTournamentRepository;
        }

        public async Task<int> GetTournamentPoolSizeAsync(int userId)
        {
            return await _movieTournamentRepository.GetTournamentPoolSizeAsync(userId);
        }

        public async Task<List<Movie>> GetTournamentPoolAsync(int userId, int poolSize)
        {
            return await _movieTournamentRepository.GetTournamentPoolAsync(userId, poolSize);
        }

        public async Task BoostMovieScoreAsync(int userId, int movieId, decimal scoreBoost)
        {
            await _movieTournamentRepository.BoostMovieScoreAsync(userId, movieId, scoreBoost);
        }
    }
}
