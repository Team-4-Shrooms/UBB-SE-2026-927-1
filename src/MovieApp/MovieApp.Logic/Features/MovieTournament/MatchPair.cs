using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.MovieTournament
{
    /// <summary>
    /// Represents a single match pairing between two movies in the tournament bracket.
    /// </summary>
    public class MatchPair
    {
        public MatchPair(Movie firstMovie, Movie? secondMovie)
        {
            this.FirstMovie = firstMovie;
            this.SecondMovie = secondMovie;
            this.WinnerMovieId = null;
        }

        public Movie FirstMovie { get; }
        public Movie? SecondMovie { get; }
        public int? WinnerMovieId { get; private set; }

        public void RecordWinner(int winnerMovieId) => this.WinnerMovieId = winnerMovieId;
        public bool IsCompleted() => this.WinnerMovieId != null;
        public bool IsByeMatch() => this.SecondMovie == null;
    }
}
