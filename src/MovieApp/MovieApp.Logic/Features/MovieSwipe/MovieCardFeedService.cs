using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;

namespace MovieApp.Logic.Features.MovieSwipe
{
    /// <summary>
    /// Implementation of the movie feed service that delegates data retrieval to the repository.
    /// Owner: Bogdan
    /// </summary>
    public class MovieCardFeedService : IMovieCardFeedService
    {
        private readonly IPreferenceRepository repository;

        public MovieCardFeedService(IPreferenceRepository repository)
        {
            this.repository = repository;
        }

        public Task<List<Movie>> FetchMovieFeedAsync(int userId, int count)
        {
            return repository.GetMovieFeedAsync(userId, count);
        }
    }
}
