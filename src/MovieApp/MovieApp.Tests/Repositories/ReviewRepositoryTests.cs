using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class ReviewRepositoryTests
    {
        [Fact]
        public async Task GetReviewsForMovieAsync_TwoReviewsExist_ReturnsTwoReviews()
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
            List<MovieReview> reviews = await repository.GetReviewsForMovieAsync(movie.Id);

            Assert.Equal(2, reviews.Count);
        }

        [Fact]
        public async Task GetReviewsForMovieAsync_TwoReviewsExist_FirstReviewIsNewerThanSecond()
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
            List<MovieReview> reviews = await repository.GetReviewsForMovieAsync(movie.Id);

            Assert.True(reviews[0].CreatedAt >= reviews[1].CreatedAt);
        }

        [Fact]
        public async Task GetReviewsForMovieAsync_NoReviewsExist_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            context.Movies.Add(movie);
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            List<MovieReview> reviews = await repository.GetReviewsForMovieAsync(movie.Id);

            Assert.Empty(reviews);
        }

        [Fact]
        public async Task GetRawRatingsForMovieAsync_TwoReviewsExist_ReturnsTwoRatings()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            User user = BuildUser();
            context.Movies.Add(movie);
            context.Users.Add(user);
            context.MovieReviews.AddRange(
                BuildReview(movie, user, 8, DateTime.UtcNow),
                BuildReview(movie, user, 5, DateTime.UtcNow));
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            List<decimal> ratings = await repository.GetRawRatingsForMovieAsync(movie.Id);

            Assert.Equal(2, ratings.Count);
        }

        [Fact]
        public async Task GetRawRatingsForMovieAsync_NoReviewsExist_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            context.Movies.Add(movie);
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            List<decimal> ratings = await repository.GetRawRatingsForMovieAsync(movie.Id);

            Assert.Empty(ratings);
        }

        [Fact]
        public async Task GetReviewCountsAsync_EmptyMovieIdList_ReturnsEmptyDictionary()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            ReviewRepository repository = new ReviewRepository(context);

            Dictionary<int, int> reviewCounts = await repository.GetReviewCountsAsync(Enumerable.Empty<int>());

            Assert.Empty(reviewCounts);
        }

        [Fact]
        public async Task GetReviewCountsAsync_MovieWithOneReview_ReturnsCountOfOne()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            User user = BuildUser();
            context.Movies.Add(movie);
            context.Users.Add(user);
            context.MovieReviews.Add(BuildReview(movie, user, 8, DateTime.UtcNow));
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            Dictionary<int, int> reviewCounts = await repository.GetReviewCountsAsync(new List<int> { movie.Id });

            Assert.Equal(1, reviewCounts[movie.Id]);
        }

        [Fact]
        public async Task AddReviewAsync_ValidReview_CreatesOneRecord()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            User user = BuildUser();
            context.Movies.Add(movie);
            context.Users.Add(user);
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            MovieReview review = new MovieReview
            {
                Movie = movie,
                User = user,
                StarRating = 7,
                Comment = "Great movie",
                CreatedAt = DateTime.UtcNow
            };
            await repository.AddReviewAsync(review);
            await repository.SaveChangesAsync();

            Assert.Single(context.MovieReviews);
        }

        [Fact]
        public async Task AddReviewAsync_RatingOfNine_StoresCorrectStarRating()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            User user = BuildUser();
            context.Movies.Add(movie);
            context.Users.Add(user);
            context.SaveChanges();

            ReviewRepository repository = new ReviewRepository(context);
            MovieReview review = new MovieReview
            {
                Movie = movie,
                User = user,
                StarRating = 9,
                Comment = null,
                CreatedAt = DateTime.UtcNow
            };
            await repository.AddReviewAsync(review);
            await repository.SaveChangesAsync();

            Assert.Equal(9m, context.MovieReviews.Single().StarRating);
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
