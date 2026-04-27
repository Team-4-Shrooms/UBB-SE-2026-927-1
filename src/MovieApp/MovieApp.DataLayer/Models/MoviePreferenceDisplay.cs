namespace MovieApp.Logic.Models
{
    /// <summary>
    /// A display model representing a user's top-scored movie preference,
    /// enriched with the movie title. Used in the personality match feature.
    /// </summary>
    public class MoviePreferenceDisplay
    {
        /// <summary>Gets or sets the unique identifier of the movie.</summary>
        public int MovieId { get; set; }

        /// <summary>Gets or sets the title of the movie.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's preference score for this movie.</summary>
        public decimal Score { get; set; }

        /// <summary>Gets or sets a value indicating whether this is the user's highest-scored movie.</summary>
        public bool IsBestMovie { get; set; }
    }
}
