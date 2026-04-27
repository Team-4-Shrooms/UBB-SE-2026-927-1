using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    /// <summary>
    /// Defines a contract for accessing and managing movie data, including retrieval, search, ownership checks, and
    /// purchase operations.
    /// </summary>
    public interface IMovieRepository
    {
        /// <summary>
        /// Retrieves a list of all movies available in the data source.
        /// </summary>
        /// <returns>A list of <see cref="Movie"/> objects representing all movies. The list will be empty if no movies are
        /// available.</returns>
        List<Movie> GetAllMovies();

        /// <summary>
        /// Retrieves a movie with the specified identifier.
        /// </summary>
        /// <param name="movieId">The unique identifier of the movie to retrieve. Must be a non-negative value.</param>
        /// <returns>The <see cref="Movie"/> instance that matches the specified <paramref name="movieId"/>, or <see
        /// langword="null"/> if no such movie exists.</returns>
        Movie? GetMovieById(int movieId);

        /// <summary>
        /// Determines whether the specified user owns the specified movie.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check ownership for. Must be a valid user ID.</param>
        /// <param name="movieId">The unique identifier of the movie to check ownership of. Must be a valid movie ID.</param>
        /// <returns><see langword="true"/> if the user owns the movie; otherwise, <see langword="false"/>.</returns>
        bool UserOwnsMovie(int userId, int movieId);

        /// <summary>
        /// Processes the purchase of a movie for a specified user at the given final price.
        /// </summary>
        /// <param name="userId">The unique identifier of the user making the purchase. Must correspond to an existing user.</param>
        /// <param name="movieId">The unique identifier of the movie to be purchased. Must correspond to an available movie.</param>
        /// <param name="finalPrice">The final price to be charged for the movie purchase. Must be a non-negative value.</param>
        void PurchaseMovie(int userId, int movieId, decimal finalPrice);

        /// <summary>
        /// Queries the database for the top 10 movies matching a partial title.
        /// </summary>
        Task<List<Movie>> SearchTop10MoviesAsync(string partialMovieName);
    }
}
