namespace MovieApp.Features.PersonalityMatch.Models
{
    /// <summary>
    /// Represents a lightweight view of a user account, containing only the essential identity and
    /// social media information needed for personality match operations.
    /// </summary>
    public class UserAccountModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Facebook account associated with the user.
        /// </summary>
        public string FacebookAccount { get; set; } = string.Empty;
    }
}