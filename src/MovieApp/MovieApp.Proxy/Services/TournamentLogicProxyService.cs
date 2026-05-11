using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.MovieTournament;

namespace MovieApp.Proxy.Services
{
    /// <summary>
    /// Proxy implementation of ITournamentLogicService.
    /// Delegates to the real TournamentLogicService backed by MovieTournamentProxyRepository.
    /// Tournament bracket state is held in memory locally (same as the real service);
    /// only pool/boost data fetching goes over HTTP.
    /// </summary>
    public class TournamentLogicProxyService : ITournamentLogicService
    {
        private readonly TournamentLogicService _inner;

        public TournamentLogicProxyService(ApiClient apiClient)
        {
            _inner = new TournamentLogicService(new MovieTournamentProxyRepository(apiClient));
        }

        public TournamentState CurrentState => _inner.CurrentState;

        public bool IsTournamentActive => _inner.IsTournamentActive;

        public Task StartTournamentAsync(int userId, int poolSize)
            => _inner.StartTournamentAsync(userId, poolSize);

        public Task AdvanceWinnerAsync(int userId, int winnerId)
            => _inner.AdvanceWinnerAsync(userId, winnerId);

        public void ResetTournament() => _inner.ResetTournament();

        public MatchPair? GetCurrentMatch() => _inner.GetCurrentMatch();

        public bool IsTournamentComplete() => _inner.IsTournamentComplete();

        public Movie GetFinalWinner() => _inner.GetFinalWinner();

        public Task<MatchPair?> GetCurrentMatchAsync(int userId)
            => _inner.GetCurrentMatchAsync(userId);

        public Task<bool> IsTournamentCompleteAsync(int userId)
            => _inner.IsTournamentCompleteAsync(userId);

        public Task<Movie> GetFinalWinnerAsync(int userId)
            => _inner.GetFinalWinnerAsync(userId);

        public Task ResetTournamentAsync(int userId)
            => _inner.ResetTournamentAsync(userId);
    }
}
