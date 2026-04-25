using System;

namespace MovieApp.Logic.Models
{
    /// <summary>
    /// Represents a user's stored preference score for a specific movie,
    /// including when it was last updated and how much it changed.
    /// </summary>
    public class UserMoviePreference
    {
        /// <summary>
        /// Gets or sets the unique identifier for this user-movie preference record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who owns this preference.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the movie this preference refers to.
        /// </summary>
        public Movie Movie { get; set; }

        /// <summary>
        /// Gets or sets the preference score assigned to this movie by the user.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this preference was last modified.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the difference between the current score and the previous score
        /// at the time of the last update.
        /// Returns <see langword="null"/> if no previous value exists.
        /// </summary>
        public int? ChangeFromPreviousValue { get; set; }
    }
}