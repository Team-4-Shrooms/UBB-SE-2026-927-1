using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.MovieTournament
{
    /// <summary>
    /// Implements the tournament bracket logic.
    /// </summary>
    public class TournamentLogicService : ITournamentLogicService
    {
        private const int MinimumPoolSize = 4;
        private const double FinalWinnerScoreBoost = 2.0;

        private readonly IMovieTournamentRepository tournamentRepository;
        private readonly Random randomNumberGenerator;
        private TournamentState? activeTournamentState;

        public TournamentLogicService(IMovieTournamentRepository tournamentRepository)
        {
            this.tournamentRepository = tournamentRepository;
            this.randomNumberGenerator = new Random();
        }

        public TournamentState CurrentState =>
            this.activeTournamentState ?? throw new InvalidOperationException("No active tournament.");

        public bool IsTournamentActive =>
            this.activeTournamentState != null && this.activeTournamentState.PendingMatches.Count > 0;

        public async Task StartTournamentAsync(int userId, int poolSize)
        {
            if (poolSize < MinimumPoolSize)
                throw new ArgumentException($"Pool size must be at least {MinimumPoolSize}.");

            List<Movie> movies = await this.tournamentRepository.GetTournamentPoolAsync(userId, poolSize);

            if (movies.Count < poolSize)
                throw new InvalidOperationException($"Not enough movies. Requested {poolSize}, but found {movies.Count}.");

            this.activeTournamentState = new TournamentState();
            this.ShuffleMovies(movies);
            this.GenerateMatchesFromMovieList(movies);
        }

        public async Task AdvanceWinnerAsync(int userId, int winnerId)
        {
            if (this.activeTournamentState == null || this.activeTournamentState.PendingMatches.Count == 0)
                throw new InvalidOperationException("No pending matches to advance.");

            MatchPair currentMatch = this.activeTournamentState.PendingMatches[0];

            bool winnerIsFirstMovie = currentMatch.FirstMovie.Id == winnerId;
            bool winnerIsSecondMovie = currentMatch.SecondMovie != null && currentMatch.SecondMovie.Id == winnerId;

            if (!winnerIsFirstMovie && !winnerIsSecondMovie)
                throw new ArgumentException("Winner ID does not match any movie in the current match.");

            currentMatch.RecordWinner(winnerId);
            this.activeTournamentState.PendingMatches.RemoveAt(0);
            this.activeTournamentState.CompletedMatches.Add(currentMatch);

            Movie winnerMovie = winnerIsFirstMovie ? currentMatch.FirstMovie : currentMatch.SecondMovie!;
            this.activeTournamentState.CurrentRoundWinners.Add(winnerMovie);

            if (this.activeTournamentState.PendingMatches.Count == 0
                && this.activeTournamentState.CurrentRoundWinners.Count > 1)
            {
                this.GenerateNextRound();
            }

            if (this.IsTournamentComplete())
            {
                Movie finalWinner = this.GetFinalWinner();
                await this.tournamentRepository.BoostMovieScoreAsync(userId, finalWinner.Id, (decimal)FinalWinnerScoreBoost);
            }
        }

        public MatchPair? GetCurrentMatch() => this.activeTournamentState?.PendingMatches.FirstOrDefault();

        public bool IsTournamentComplete() =>
            this.activeTournamentState != null
            && this.activeTournamentState.PendingMatches.Count == 0
            && this.activeTournamentState.CurrentRoundWinners.Count == 1;

        public Movie GetFinalWinner()
        {
            if (!this.IsTournamentComplete())
                throw new InvalidOperationException("Tournament is not yet complete.");
            return this.activeTournamentState!.CurrentRoundWinners[0];
        }

        public void ResetTournament() => this.activeTournamentState = null;

        private void GenerateNextRound()
        {
            List<Movie> roundWinners = new List<Movie>(this.activeTournamentState!.CurrentRoundWinners);
            this.activeTournamentState.CurrentRoundWinners.Clear();
            this.activeTournamentState.CurrentRound++;
            this.ShuffleMovies(roundWinners);
            this.GenerateMatchesFromMovieList(roundWinners);
        }

        private void ShuffleMovies(List<Movie> movies)
        {
            for (int index = movies.Count - 1; index > 0; index--)
            {
                int swapIndex = this.randomNumberGenerator.Next(index + 1);
                (movies[index], movies[swapIndex]) = (movies[swapIndex], movies[index]);
            }
        }

        private void GenerateMatchesFromMovieList(List<Movie> movies)
        {
            int pairCount = movies.Count / 2;
            bool hasByeMatch = movies.Count % 2 != 0;

            for (int index = 0; index < pairCount; index++)
            {
                this.activeTournamentState!.PendingMatches.Add(
                    new MatchPair(movies[index * 2], movies[(index * 2) + 1]));
            }

            if (hasByeMatch)
            {
                Movie byeMovie = movies.Last();
                MatchPair byeMatch = new MatchPair(byeMovie, null);
                byeMatch.RecordWinner(byeMovie.Id);
                this.activeTournamentState!.CompletedMatches.Add(byeMatch);
                this.activeTournamentState.CurrentRoundWinners.Add(byeMovie);
            }
        }
    }
}
