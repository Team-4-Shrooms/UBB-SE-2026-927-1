namespace MovieApp.Features.PersonalityMatch.Models
{
    /// <summary>
    /// Represents the result of a personality match between the current user and
    /// another user, including the match score and relevant profile information.
    /// </summary>
    public class MatchResult
    {
        /// <summary>
        /// Gets or sets the unique identifier of the matched user.
        /// </summary>
        public int MatchedUserId { get; set; }

        /// <summary>
        /// Gets or sets the username of the matched user.
        /// </summary>
        public string MatchedUsername { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the compatibility score between the current user and the matched user.
        /// Higher values indicate a stronger personality match.
        /// </summary>
        public double MatchScore { get; set; }

        /// <summary>
        /// Gets or sets the Facebook account associated with the matched user.
        /// </summary>
        public string FacebookAccount { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this result is being viewed by the matched user themselves.
        /// Returns <see langword="true"/> if the viewer is the same as the matched user; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsSelfView { get; set; } = false;
    }
}