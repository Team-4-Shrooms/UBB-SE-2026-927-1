using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    /// <summary>
    /// Defines a contract for accessing and managing movie data, including retrieval, search, ownership checks, and
    /// purchase operations.
    /// </summary>
    public interface IMovieRepository
    {
        Task<Movie?> GetMovieByIdAsync(int movieId);
        Task<List<Movie>> SearchMoviesAsync(string query, int limit);
        Task<bool> UserOwnsMovieAsync(int userId, int movieId);
        Task AddOwnedMovieAsync(OwnedMovie ownership);
        Task AddTransactionAsync(Transaction transaction);
        Task<int> SaveChangesAsync();
    }
}
