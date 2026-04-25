using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        List<MovieReview> GetReviewsForMovie(int movieId);

        void AddReview(int movieId, int userId, int starRating, string? comment);

        int GetReviewCount(int movieId);

        Dictionary<int, int> GetReviewCounts(IEnumerable<int> movieIds);

        int[] GetStarRatingBuckets(int movieId);
    }
}
