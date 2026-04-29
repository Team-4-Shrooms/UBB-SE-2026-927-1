using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Features.MovieTournament.Models;

namespace MovieApp.Features.MovieTournament.Services
{
    /// <summary>
    /// Implements the tournament bracket logic, including pool shuffling,
    /// match generation, winner advancement, and bye-match handling.
    /// </summary>
    public class TournamentLogicService : ITournamentLogicService
    {
        private const int MinimumPoolSize = 4;
        private const double FinalWinnerScoreBoost = 2.0;

        private readonly IMovieTournamentRepository tournamentRepository;
        private readonly Random randomNumberGenerator;
        private TournamentState? activeTournamentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentLogicService"/> class.
        /// </summary>
        /// <param name="tournamentRepository">
        /// The repository used to fetch movie pools and persist score boosts.
        /// </param>
        public TournamentLogicService(IMovieTournamentRepository tournamentRepository)
        {
            this.tournamentRepository = tournamentRepository;
            this.randomNumberGenerator = new Random();
        }

        /// <inheritdoc/>
        public TournamentState CurrentState =>
            this.activeTournamentState ?? throw new InvalidOperationException("No active tournament.");

        /// <inheritdoc/>
        public bool IsTournamentActive =>
            this.activeTournamentState != null && this.activeTournamentState.PendingMatches.Count > 0;

        /// <inheritdoc/>
        public async Task StartTournamentAsync(int userId, int poolSize)
        {
            if (poolSize < MinimumPoolSize)
            {
                throw new ArgumentException($"Pool size must be at least {MinimumPoolSize}.");
            }

            List<Movie> movies = await this.tournamentRepository.GetTournamentPoolAsync(userId, poolSize);

            if (movies.Count < poolSize)
            {
                throw new InvalidOperationException(
                    $"Not enough movies. Requested {poolSize}, but found {movies.Count}.");
            }

            this.activeTournamentState = new TournamentState();

            this.ShuffleMovies(movies);
            this.GenerateMatchesFromMovieList(movies);
        }

        /// <inheritdoc/>
        public async Task AdvanceWinnerAsync(int userId, int winnerId)
        {
            if (this.activeTournamentState == null || this.activeTournamentState.PendingMatches.Count == 0)
            {
                throw new InvalidOperationException("No pending matches to advance.");
            }

            MatchPair currentMatch = this.activeTournamentState.PendingMatches[0];

            bool winnerIsFirstMovie = currentMatch.FirstMovie.Id == winnerId;
            bool winnerIsSecondMovie = currentMatch.SecondMovie != null && currentMatch.SecondMovie.Id == winnerId;

            if (!winnerIsFirstMovie && !winnerIsSecondMovie)
            {
                throw new ArgumentException("Winner ID does not match any movie in the current match.");
            }

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

        /// <inheritdoc/>
        public MatchPair? GetCurrentMatch()
        {
            return this.activeTournamentState?.PendingMatches.FirstOrDefault();
        }

        /// <inheritdoc/>
        public bool IsTournamentComplete()
        {
            return this.activeTournamentState != null
                && this.activeTournamentState.PendingMatches.Count == 0
                && this.activeTournamentState.CurrentRoundWinners.Count == 1;
        }

        /// <inheritdoc/>
        public Movie GetFinalWinner()
        {
            if (!this.IsTournamentComplete())
            {
                throw new InvalidOperationException("Tournament is not yet complete.");
            }

            return this.activeTournamentState!.CurrentRoundWinners[0];
        }

        /// <inheritdoc/>
        public void ResetTournament()
        {
            this.activeTournamentState = null;
        }

        /// <summary>
        /// Advances the tournament to the next round by shuffling the current round's
        /// winners and generating a new set of matches from them.
        /// </summary>
        private void GenerateNextRound()
        {
            List<Movie> roundWinners = new List<Movie>(this.activeTournamentState!.CurrentRoundWinners);
            this.activeTournamentState.CurrentRoundWinners.Clear();
            this.activeTournamentState.CurrentRound++;

            this.ShuffleMovies(roundWinners);
            this.GenerateMatchesFromMovieList(roundWinners);
        }

        /// <summary>
        /// Shuffles a list of movies in place using the Fisher-Yates algorithm.
        /// </summary>
        /// <param name="movies">The list of movies to shuffle.</param>
        private void ShuffleMovies(List<Movie> movies)
        {
            for (int index = movies.Count - 1; index > 0; index--)
            {
                int swapIndex = this.randomNumberGenerator.Next(index + 1);
                (movies[index], movies[swapIndex]) = (movies[swapIndex], movies[index]);
            }
        }

        /// <summary>
        /// Generates match pairs from a list of movies and adds them to the active tournament state.
        /// If the list has an odd number of movies, the last movie receives a bye
        /// and advances automatically to the next round.
        /// </summary>
        /// <param name="movies">The list of movies to pair into matches.</param>
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
