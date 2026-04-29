using System.Collections.Generic;
using MovieApp.DataLayer.Models;

namespace MovieApp.Features.MovieTournament.Models
{
    /// <summary>
    /// Represents the current state of a movie tournament,
    /// including all pending and completed matches, the active round number,
    /// and the list of winners advancing to the next round.
    /// </summary>
    public class TournamentState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentState"/> class,
        /// starting at round one with empty match and winner lists.
        /// </summary>
        public TournamentState()
        {
            this.PendingMatches = new List<MatchPair>();
            this.CompletedMatches = new List<MatchPair>();
            this.CurrentRoundWinners = new List<Movie>();
            this.CurrentRound = 1;
        }

        /// <summary>
        /// Gets or sets the list of matches that have not yet been played in the current round.
        /// </summary>
        public List<MatchPair> PendingMatches { get; set; }

        /// <summary>
        /// Gets or sets the list of matches that have already been played and have a recorded winner.
        /// </summary>
        public List<MatchPair> CompletedMatches { get; set; }

        /// <summary>
        /// Gets or sets the one-based number of the round currently being played.
        /// </summary>
        public int CurrentRound { get; set; }

        /// <summary>
        /// Gets or sets the list of movies that have won their match in the current round
        /// and will advance to the next round.
        /// </summary>
        public List<Movie> CurrentRoundWinners { get; set; }
    }
}
