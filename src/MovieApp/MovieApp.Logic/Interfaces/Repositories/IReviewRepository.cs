using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    /// <summary>
    /// A repository interface for managing movie reviews.
    /// </summary>
    public interface IReviewRepository
    {
        Task<List<MovieReview>> GetReviewsForMovieAsync(int movieId);
        Task<List<decimal>> GetRawRatingsForMovieAsync(int movieId);
        Task<Dictionary<int, int>> GetReviewCountsAsync(IEnumerable<int> movieIds);
        Task AddReviewAsync(MovieReview review);
        Task<int> SaveChangesAsync();
    }
}
