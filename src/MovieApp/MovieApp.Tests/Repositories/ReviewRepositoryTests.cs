using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class ReviewRepositoryTests
    {
        [Fact]
        public void GetReviewsForMovie_ReturnsReviewsOrderedDescending()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            User user = BuildUser();
            context.Movies.Add(movie);
            context.Users.Add(user);
            context.MovieReviews.AddRange(
                BuildReview(movie, user, 8, DateTime.UtcNow.AddDays(-2)),
                BuildReview(movie, user, 9, DateTime.UtcNow));
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            List<MovieReview> result = repository.GetReviewsForMovie(movie.Id);

            Assert.Equal(2, result.Count);
            Assert.True(result[0].CreatedAt >= result[1].CreatedAt);
        }

        [Fact]
        public void AddReview_AddsReviewWithSanitizedComment()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            User user = BuildUser();
            context.Movies.Add(movie);
            context.Users.Add(user);
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            repository.AddReview(movie.Id, user.Id, 7, "   ");

            MovieReview stored = Assert.Single(context.MovieReviews);
            Assert.Equal(7m, stored.StarRating);
            Assert.Null(stored.Comment);
        }

        [Fact]
        public void AddReview_PreservesNonEmptyComment()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            User user = BuildUser();
            context.Movies.Add(movie);
            context.Users.Add(user);
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            repository.AddReview(movie.Id, user.Id, 9, "Great");

            Assert.Equal("Great", context.MovieReviews.Single().Comment);
        }

        [Fact]
        public void GetReviewCount_ReturnsCorrectCount()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            User user = BuildUser();
            context.Movies.Add(movie);
            context.Users.Add(user);
            context.MovieReviews.AddRange(
                BuildReview(movie, user, 8, DateTime.UtcNow),
                BuildReview(movie, user, 9, DateTime.UtcNow));
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);

            Assert.Equal(2, repository.GetReviewCount(movie.Id));
        }

        [Fact]
        public void GetReviewCounts_EmptyIds_ReturnsEmptyDictionary()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            ReviewRepository repository = new ReviewRepository(context);

            Dictionary<int, int> result = repository.GetReviewCounts(Enumerable.Empty<int>());

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetReviewCounts_DeduplicatesIds()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            User user = BuildUser();
            context.Movies.Add(movie);
            context.Users.Add(user);
            context.MovieReviews.Add(BuildReview(movie, user, 8, DateTime.UtcNow));
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            Dictionary<int, int> result = repository.GetReviewCounts(new List<int> { movie.Id, movie.Id });

            Assert.Single(result);
            Assert.Equal(1, result[movie.Id]);
        }

        [Fact]
        public void GetStarRatingBuckets_BucketsAreClampedAndIndexed()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            User user = BuildUser();
            context.Movies.Add(movie);
            context.Users.Add(user);
            context.MovieReviews.AddRange(
                BuildReview(movie, user, 1, DateTime.UtcNow),
                BuildReview(movie, user, 1, DateTime.UtcNow),
                BuildReview(movie, user, 5, DateTime.UtcNow),
                BuildReview(movie, user, 10, DateTime.UtcNow));
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            int[] buckets = repository.GetStarRatingBuckets(movie.Id);

            Assert.Equal(11, buckets.Length);
            Assert.Equal(0, buckets[0]);
            Assert.Equal(2, buckets[1]);
            Assert.Equal(1, buckets[5]);
            Assert.Equal(1, buckets[10]);
        }

        private static Movie BuildMovie()
        {
            return new Movie
            {
                Title = "Inception",
                Description = "desc",
                PrimaryGenre = "Drama",
                Synopsis = "synopsis",
                Price = 10m,
                Rating = 0m,
                ReleaseYear = 2010
            };
        }

        private static User BuildUser()
        {
            return new User
            {
                Username = "user",
                Email = "user@example.com",
                PasswordHash = "hash",
                Balance = 100m
            };
        }

        private static MovieReview BuildReview(Movie movie, User user, decimal rating, DateTime createdAt)
        {
            return new MovieReview
            {
                Movie = movie,
                User = user,
                StarRating = rating,
                Comment = null,
                CreatedAt = createdAt
            };
        }
    }
}
