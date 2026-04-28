using MovieApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for accessing and updating movie data used during tournament play.
    /// </summary>
    public interface IMovieTournamentRepository
    {
        /// <summary>
        /// Retrieves the number of movies in the tournament pool for a given user.
        /// The pool consists of movies the user has positively interacted with.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <returns>The total number of movies available in the user's tournament pool.</returns>
        Task<int> GetTournamentPoolSizeAsync(int userId);

        /// <summary>
        /// Retrieves the list of movies that form the tournament pool for a given user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="poolSize">The maximum number of movies to include in the pool.</param>
        /// <returns>A list of <see cref="Movie"/> representing the tournament pool.</returns>
        Task<List<Movie>> GetTournamentPoolAsync(int userId, int poolSize);

        /// <summary>
        /// Applies a score boost to a movie preference for a given user,
        /// typically awarded when the movie wins a tournament match.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="movieId">The identifier of the movie to boost.</param>
        /// <param name="scoreBoost">The amount by which to increase the movie's preference score.</param>
        Task BoostMovieScoreAsync(int userId, int movieId, decimal scoreBoost);
    }
}
