namespace MovieApp.Logic.Features.PersonalityMatch
{
    /// <summary>
    /// Represents the result of a personality match between users.
    /// </summary>
    public class MatchResult
    {
        public int MatchedUserId { get; set; }
        public string MatchedUsername { get; set; } = string.Empty;
        public double MatchScore { get; set; }
        public string FacebookAccount { get; set; } = string.Empty;
        public bool IsSelfView { get; set; } = false;
    }
}
