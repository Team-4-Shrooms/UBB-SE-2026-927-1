using MovieApp.Logic.Models;

namespace MovieApp.Features.MovieTournament.Models
{
    /// <summary>
    /// Represents a single match pairing between two movies in the tournament bracket.
    /// A match may be a bye match if only one movie is present in the pairing.
    /// </summary>
    public class MatchPair
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchPair"/> class.
        /// </summary>
        /// <param name="firstMovie">The first movie in the match.</param>
        /// <param name="secondMovie">
        /// The second movie in the match, or <see langword="null"/> if this is a bye match.
        /// </param>
        public MatchPair(Movie firstMovie, Movie? secondMovie)
        {
            this.FirstMovie = firstMovie;
            this.SecondMovie = secondMovie;
            this.WinnerMovieId = null;
        }

        /// <summary>
        /// Gets the first movie competing in this match.
        /// </summary>
        public Movie FirstMovie { get; }

        /// <summary>
        /// Gets the second movie competing in this match.
        /// Returns <see langword="null"/> when this is a bye match.
        /// </summary>
        public Movie? SecondMovie { get; }

        /// <summary>
        /// Gets the identifier of the movie that won this match.
        /// Returns <see langword="null"/> if the match has not been completed yet.
        /// </summary>
        public int? WinnerMovieId { get; private set; }

        /// <summary>
        /// Records the winner of this match by storing the winning movie's identifier.
        /// </summary>
        /// <param name="winnerMovieId">The identifier of the winning movie.</param>
        public void RecordWinner(int winnerMovieId)
        {
            this.WinnerMovieId = winnerMovieId;
        }

        /// <summary>
        /// Determines whether this match has been completed,
        /// meaning a winner has been recorded.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if a winner has been recorded; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsCompleted()
        {
            return this.WinnerMovieId != null;
        }

        /// <summary>
        /// Determines whether this match is a bye match,
        /// meaning the first movie advances automatically with no opponent.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if there is no second movie; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsByeMatch()
        {
            return this.SecondMovie == null;
        }
    }
}