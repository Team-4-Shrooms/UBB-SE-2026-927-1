using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class ReviewRepository : IReviewRepository
    {
        private const int StarRatingBucketCount = 11;
        private const int MinStarRating = 1;
        private const int MaxStarRating = 10;

        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<MovieReview> GetReviewsForMovie(int movieId)
        {
            return _context.MovieReviews
                .AsNoTracking()
                .Include(review => review.User)
                .Where(review => review.Movie.Id == movieId)
                .OrderByDescending(review => review.CreatedAt)
                .ThenByDescending(review => review.Id)
                .ToList();
        }

        public void AddReview(int movieId, int userId, int starRating, string? comment)
        {
            Movie? movie = _context.Movies.FirstOrDefault(candidate => candidate.Id == movieId);
            if (movie is null)
            {
                throw new InvalidOperationException("Movie not found.");
            }

            User? user = _context.Users.FirstOrDefault(candidate => candidate.Id == userId);
            if (user is null)
            {
                throw new InvalidOperationException("User not found.");
            }

            string? sanitizedComment = string.IsNullOrWhiteSpace(comment) ? null : comment;

            MovieReview review = new MovieReview
            {
                Movie = movie,
                User = user,
                StarRating = starRating,
                Comment = sanitizedComment,
                CreatedAt = DateTime.UtcNow
            };
            _context.MovieReviews.Add(review);
            _context.SaveChanges();
        }

        public int GetReviewCount(int movieId)
        {
            return _context.MovieReviews.Count(review => review.Movie.Id == movieId);
        }

        public Dictionary<int, int> GetReviewCounts(IEnumerable<int> movieIds)
        {
            if (movieIds is null)
            {
                return new Dictionary<int, int>();
            }

            List<int> distinctIds = movieIds.Distinct().ToList();
            if (distinctIds.Count == 0)
            {
                return new Dictionary<int, int>();
            }

            return _context.MovieReviews
                .Where(review => distinctIds.Contains(review.Movie.Id))
                .GroupBy(review => review.Movie.Id)
                .Select(group => new { MovieId = group.Key, Count = group.Count() })
                .ToDictionary(entry => entry.MovieId, entry => entry.Count);
        }

        public int[] GetStarRatingBuckets(int movieId)
        {
            int[] counts = new int[StarRatingBucketCount];

            List<decimal> ratings = _context.MovieReviews
                .Where(review => review.Movie.Id == movieId)
                .Select(review => review.StarRating)
                .ToList();

            foreach (decimal rating in ratings)
            {
                int bucket = (int)Math.Floor((double)rating);
                if (bucket < MinStarRating)
                {
                    bucket = MinStarRating;
                }

                if (bucket > MaxStarRating)
                {
                    bucket = MaxStarRating;
                }

                counts[bucket]++;
            }

            return counts;
        }
    }
}
