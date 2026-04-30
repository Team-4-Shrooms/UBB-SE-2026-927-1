using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.MovieTournament
{
    /// <summary>
    /// Defines the contract for the service that manages tournament logic.
    /// </summary>
    public interface ITournamentLogicService
    {
        TournamentState CurrentState { get; }
        bool IsTournamentActive { get; }
        Task StartTournamentAsync(int userId, int poolSize);
        Task AdvanceWinnerAsync(int userId, int winnerId);
        void ResetTournament();
        MatchPair? GetCurrentMatch();
        bool IsTournamentComplete();
        Movie GetFinalWinner();
    }
}
