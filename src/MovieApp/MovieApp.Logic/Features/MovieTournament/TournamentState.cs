using System.Collections.Generic;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.MovieTournament
{
    /// <summary>
    /// Represents the current state of a movie tournament.
    /// </summary>
    public class TournamentState
    {
        public TournamentState()
        {
            this.PendingMatches = new List<MatchPair>();
            this.CompletedMatches = new List<MatchPair>();
            this.CurrentRoundWinners = new List<Movie>();
            this.CurrentRound = 1;
        }

        public List<MatchPair> PendingMatches { get; set; }
        public List<MatchPair> CompletedMatches { get; set; }
        public int CurrentRound { get; set; }
        public List<Movie> CurrentRoundWinners { get; set; }
    }
}
