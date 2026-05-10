using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.MovieTournament;

namespace MovieApp.Proxy.Services
{
    public class TournamentLogicProxyService : ITournamentLogicService
    {
        private readonly ApiClient _apiClient;

        public TournamentLogicProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public TournamentState CurrentState => throw new NotSupportedException("Use async methods.");
        public bool IsTournamentActive => throw new NotSupportedException("Use async methods.");
        public void ResetTournament() => throw new NotSupportedException("Use async methods.");
        public MatchPair? GetCurrentMatch() => throw new NotSupportedException("Use async methods.");
        public bool IsTournamentComplete() => throw new NotSupportedException("Use async methods.");
        public Movie GetFinalWinner() => throw new NotSupportedException("Use async methods.");

        public async Task StartTournamentAsync(int userId, int poolSize)
        {
            await _apiClient.PostAsync($"api/tournament/{userId}/start?poolSize={poolSize}", new { });
        }

        public async Task AdvanceWinnerAsync(int userId, int winnerId)
        {
            await _apiClient.PostAsync($"api/tournament/{userId}/advance?winnerId={winnerId}", new { });
        }

        public async Task<MatchPair?> GetCurrentMatchAsync(int userId)
        {
            return await _apiClient.GetAsync<MatchPair>($"api/tournament/{userId}/current-match");
        }

        public async Task<bool> IsTournamentCompleteAsync(int userId)
        {
            return await _apiClient.GetAsync<bool>($"api/tournament/{userId}/is-complete");
        }

        public async Task<Movie> GetFinalWinnerAsync(int userId)
        {
            var result = await _apiClient.GetAsync<Movie>($"api/tournament/{userId}/final-winner");
            return result ?? new Movie();
        }

        public async Task ResetTournamentAsync(int userId)
        {
            await _apiClient.PostAsync($"api/tournament/{userId}/reset", new { });
        }
    }
}
