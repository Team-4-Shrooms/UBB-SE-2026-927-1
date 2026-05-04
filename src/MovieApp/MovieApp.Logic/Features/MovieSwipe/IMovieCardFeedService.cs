using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.MovieSwipe
{
    /// <summary>
    /// Service for fetching the movie feed.
    /// </summary>
    public interface IMovieCardFeedService
    {
        Task<List<Movie>> FetchMovieFeedAsync(int userId, int count);
    }
}
