using MovieApp.DataLayer.Models;

namespace MovieApp.Web.Models
{
    public class TournamentSetupViewModel
    {
        public int PoolSize { get; set; } = 8; 
    }

    public class TournamentMatchViewModel
    {
        public Movie Left { get; set; } = null!;
        public Movie Right { get; set; } = null!;
        public int CurrentRound { get; set; }
        public int TotalRounds { get; set; }
    }

    public class TournamentWinnerViewModel
    {
        public Movie Winner { get; set; } = null!;
    }
}
