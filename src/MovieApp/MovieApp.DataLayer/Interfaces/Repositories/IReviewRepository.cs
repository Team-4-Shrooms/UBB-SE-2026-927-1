using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// A repository interface for managing movie reviews.
    /// </summary>
    public interface IReviewRepository
    {
        /// <summary>
        /// Retrieves all reviews for a given movie, including the reviewer's username, star rating, and comment.
        /// </summary>
        /// <param name="movieId">The movieId for what to retreive all reviews.</param>
        /// <returns>The list of reviews for the specified movie.</returns>
        List<MovieReview> GetReviewsForMovie(int movieId);

        /// <summary>
        /// The add review method takes in the movieId, userId, starRating, and an optional comment. 
        /// It adds a new review to the database for the specified movie and user. 
        /// The starRating should be an integer value (eg, from 1 to 10), and the comment can be a string that provides additional feedback about the movie. 
        /// </summary>
        /// <param name="movieId">An integer representing the ID of the movie being reviewed.</param>
        /// <param name="userId">An integer representing the ID of the user submitting the review.</param>
        /// <param name="starRating">An integer representing the star rating given to the movie.</param>
        /// <param name="comment">An optional string containing the user's comment or feedback about the movie.</param>
        void AddReview(int movieId, int userId, int starRating, string? comment);

        /// <summary>
        /// Gets the total number of reviews associated with the specified movie.
        /// </summary>
        /// <param name="movieId">The unique identifier of the movie for which to retrieve the review count. 
        /// Must be a valid movie ID.</param>
        /// <returns>The number of reviews for the specified movie.</returns>
        int GetReviewCount(int movieId);

        /// <summary>
        /// Retrieves the number of reviews for each specified movie.
        /// </summary>
        /// <param name="movieIds">A collection of movie identifiers for which to retrieve review counts. 
        /// Each identifier must correspond to a valid movie.</param>
        /// <returns>A dictionary mapping each movie identifier to the number of reviews it has. 
        /// Movies not found in the data source are omitted from the result.</returns>
        Dictionary<int, int> GetReviewCounts(IEnumerable<int> movieIds);

        /// <summary>
        /// Returns an array containing the count of user ratings for a movie, grouped by star rating.
        /// </summary>
        /// <param name="movieId">The unique identifier of the movie for which to retrieve rating counts.</param>
        /// <returns>An array of integers where each element represents the number of user ratings for the corresponding star value. 
        /// The array is ordered by star rating. If the movie has no ratings, all elements will be zero.</returns>
        int[] GetStarRatingBuckets(int movieId);
    }
}
