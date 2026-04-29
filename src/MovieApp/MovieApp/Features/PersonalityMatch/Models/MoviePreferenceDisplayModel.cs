namespace MovieApp.Features.PersonalityMatch.Models
{
    /// <summary>
    /// Represents a movie preference entry intended for display purposes, combining movie identity information
    /// with the user's preference score and a flag indicating whether it is the user's top-rated movie.
    /// </summary>
    public class MoviePreferenceDisplay
    {
        /// <summary>
        /// Gets or sets the unique identifier of the movie.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Gets or sets the title of the movie.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the preference score assigned to this movie by the user.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this movie is the user's highest-rated movie.
        /// Returns <see langword="true"/> if this is the best movie; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsBestMovie { get; set; }
    }
}
