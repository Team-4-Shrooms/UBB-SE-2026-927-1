using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IMovieTournamentService
    {
        Task<int> GetTournamentPoolSizeAsync(int userId);
        Task<List<Movie>> GetTournamentPoolAsync(int userId, int poolSize);
        Task BoostMovieScoreAsync(int userId, int movieId, decimal scoreBoost);
    }
}
