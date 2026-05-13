using System;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.MovieTournament;

namespace MovieApp.Proxy.Services
{
    /// <summary>
    /// Proxy implementation of ITournamentLogicService.
    /// Routes all async tournament actions to the WebApi Singleton over HTTP.
    /// </summary>
    public class TournamentLogicProxyService : ITournamentLogicService
    {
        private readonly ApiClient _apiClient;

        public TournamentLogicProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // --- HTTP PROXIED ASYNC METHODS ---

        public Task StartTournamentAsync(int userId, int poolSize)
            => _apiClient.PostAsync<object>($"/api/tournament-game/{userId}/start?poolSize={poolSize}", null);

        public Task AdvanceWinnerAsync(int userId, int winnerId)
            => _apiClient.PostAsync<object>($"/api/tournament-game/{userId}/advance", winnerId);

        public Task<MatchPair?> GetCurrentMatchAsync(int userId)
            => _apiClient.GetAsync<MatchPair?>($"/api/tournament-game/{userId}/current-match");

        public Task<bool> IsTournamentCompleteAsync(int userId)
            => _apiClient.GetAsync<bool>($"/api/tournament-game/{userId}/is-complete");

        public Task<Movie> GetFinalWinnerAsync(int userId)
            => _apiClient.GetAsync<Movie>($"/api/tournament-game/{userId}/winner");

        public Task ResetTournamentAsync(int userId)
            => _apiClient.PostAsync<object>($"/api/tournament-game/{userId}/reset", null);


        // --- SYNCHRONOUS LEGACY METHODS (Not supported over HTTP) ---

        public TournamentState CurrentState => throw new NotSupportedException("Synchronous tournament state is not supported via proxy. Use Async methods.");

        public bool IsTournamentActive => throw new NotSupportedException("Synchronous tournament state is not supported via proxy. Use Async methods.");

        public void ResetTournament() => throw new NotSupportedException("Synchronous tournament state is not supported via proxy. Use Async methods.");

        public MatchPair? GetCurrentMatch() => throw new NotSupportedException("Synchronous tournament state is not supported via proxy. Use Async methods.");

        public bool IsTournamentComplete() => throw new NotSupportedException("Synchronous tournament state is not supported via proxy. Use Async methods.");

        public Movie GetFinalWinner() => throw new NotSupportedException("Synchronous tournament state is not supported via proxy. Use Async methods.");
    }
}
