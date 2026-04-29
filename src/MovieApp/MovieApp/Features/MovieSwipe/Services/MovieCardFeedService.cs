using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;

namespace MovieApp.Features.MovieSwipe.Services
{
    /// <summary>
    /// Service for fetching the movie feed.
    /// Matches UML diagram definition.
    /// Owner: Bogdan
    /// </summary>
    public interface IMovieCardFeedService
    {
        /// <summary> Fetches a collection of movie cards specifically for the UI feed. </summary>
        /// <param name="userId">The ID of the user viewing the feed.</param>
        /// <param name="count">The number of movies to fetch.</param>
        /// <returns>A task containing the list of movie cards.</returns>
        Task<List<Movie>> FetchMovieFeedAsync(int userId, int count);
    }

    /// <summary>
    /// Implementation of the movie feed service that delegates data retrieval to the repository.
    /// </summary>
    public class MovieCardFeedService : IMovieCardFeedService
    {
        /// <summary> The repository used for data access. </summary>
        private readonly IPreferenceRepository repository;

        /// <summary> Initializes a new instance of the <see cref="MovieCardFeedService"/> class. </summary>
        /// <param name="repository">The preference repository.</param>
        public MovieCardFeedService(IPreferenceRepository repository)
        {
            this.repository = repository;
        }

        /// <inheritdoc />
        public Task<List<Movie>> FetchMovieFeedAsync(int userId, int count)
        {
            return repository.GetMovieFeedAsync(userId, count);
        }
    }
}
