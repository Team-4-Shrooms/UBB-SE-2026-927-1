using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    public interface IMovieRepository
    {
        List<Movie> GetAllMovies();

        Movie? GetMovieById(int movieId);

        bool UserOwnsMovie(int userId, int movieId);

        void PurchaseMovie(int userId, int movieId, decimal finalPrice);

        /// <summary>
        /// Queries the database for the top 10 movies matching a partial title.
        /// </summary>
        Task<List<Movie>> SearchTop10MoviesAsync(string partialMovieName);
    }
}
