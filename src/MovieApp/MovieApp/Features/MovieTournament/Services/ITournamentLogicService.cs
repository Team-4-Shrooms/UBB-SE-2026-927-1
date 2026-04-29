using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Features.MovieTournament.Models;

namespace MovieApp.Features.MovieTournament.Services
{
    /// <summary>
    /// Defines the contract for the service that manages tournament logic,
    /// including bracket progression, match resolution, and winner determination.
    /// </summary>
    public interface ITournamentLogicService
    {
        /// <summary>
        /// Gets the current state of the tournament,
        /// including pending matches, completed matches, and round winners.
        /// </summary>
        TournamentState CurrentState { get; }

        /// <summary>
        /// Gets a value indicating whether a tournament is currently in progress.
        /// </summary>
        bool IsTournamentActive { get; }

        /// <summary>
        /// Initializes and starts a new tournament for the given user
        /// by loading the tournament pool and generating the first round of matches.
        /// </summary>
        /// <param name="userId">The identifier of the user starting the tournament.</param>
        /// <param name="poolSize">The number of movies to include in the tournament pool.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StartTournamentAsync(int userId, int poolSize);

        /// <summary>
        /// Records the winner of the current match and advances the tournament,
        /// applying a score boost to the winning movie and progressing to the next match or round.
        /// </summary>
        /// <param name="userId">The identifier of the user playing the tournament.</param>
        /// <param name="winnerId">The identifier of the movie chosen as the winner.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AdvanceWinnerAsync(int userId, int winnerId);

        /// <summary>
        /// Resets the tournament to its initial state,
        /// clearing all matches, winners, and round information.
        /// </summary>
        void ResetTournament();

        /// <summary>
        /// Retrieves the current match that needs to be played.
        /// </summary>
        /// <returns>
        /// The next pending <see cref="MatchPair"/>, or <see langword="null"/>
        /// if there are no remaining matches in the current round.
        /// </returns>
        MatchPair? GetCurrentMatch();

        /// <summary>
        /// Determines whether the tournament has concluded,
        /// meaning a single final winner has been identified.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the tournament is complete; otherwise, <see langword="false"/>.
        /// </returns>
        bool IsTournamentComplete();

        /// <summary>
        /// Retrieves the movie that won the entire tournament.
        /// </summary>
        /// <returns>The <see cref="Movie"/> representing the final tournament winner.</returns>
        Movie GetFinalWinner();
    }
}