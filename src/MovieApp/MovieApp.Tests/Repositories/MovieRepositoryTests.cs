using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class MovieRepositoryTests
    {
        [Fact]
        public async Task GetMovieByIdAsync_MovieExists_ReturnsMovieWithMatchingTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie("Inception");
            context.Movies.Add(movie);
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);
            Movie? result = await repository.GetMovieByIdAsync(movie.Id);

            Assert.Equal("Inception", result!.Title);
        }

        [Fact]
        public async Task GetMovieByIdAsync_MovieDoesNotExist_ReturnsNull()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            MovieRepository repository = new MovieRepository(context);

            Movie? result = await repository.GetMovieByIdAsync(123);

            Assert.Null(result);
        }

        [Fact]
        public async Task UserOwnsMovieAsync_UserOwnsTheMovie_ReturnsTrue()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser("buyer", 100m);
            Movie movie = BuildMovie("Inception");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.OwnedMovies.Add(new OwnedMovie { User = user, Movie = movie });
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);

            bool ownsMovie = await repository.UserOwnsMovieAsync(user.Id, movie.Id);

            Assert.True(ownsMovie);
        }

        [Fact]
        public async Task UserOwnsMovieAsync_UserDoesNotOwnTheMovie_ReturnsFalse()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser("buyer", 100m);
            Movie movie = BuildMovie("Inception");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);

            bool ownsMovie = await repository.UserOwnsMovieAsync(user.Id, movie.Id);

            Assert.False(ownsMovie);
        }

        [Fact]
        public async Task SearchMoviesAsync_PartialTitleMatch_ReturnsCorrectCount()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            context.Movies.AddRange(
                BuildMovie("The Matrix"),
                BuildMovie("The Matrix Reloaded"),
                BuildMovie("Inception"));
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);
            List<Movie> matchingMovies = await repository.SearchMoviesAsync("Matrix", 10);

            Assert.Equal(2, matchingMovies.Count);
        }

        [Fact]
        public async Task SearchMoviesAsync_PartialTitleMatch_AllResultsContainSearchTerm()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            context.Movies.AddRange(
                BuildMovie("The Matrix"),
                BuildMovie("The Matrix Reloaded"),
                BuildMovie("Inception"));
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);
            List<Movie> matchingMovies = await repository.SearchMoviesAsync("Matrix", 10);

            bool allContainSearchTerm = matchingMovies.All(movie => movie.Title.Contains("Matrix"));

            Assert.True(allContainSearchTerm);
        }

        [Fact]
        public async Task SearchMoviesAsync_LimitSetToTwo_ReturnsAtMostTwoResults()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            context.Movies.AddRange(
                BuildMovie("The Matrix"),
                BuildMovie("The Matrix Reloaded"),
                BuildMovie("The Matrix Revolutions"));
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);
            List<Movie> matchingMovies = await repository.SearchMoviesAsync("Matrix", 2);

            Assert.Equal(2, matchingMovies.Count);
        }

        [Fact]
        public async Task AddOwnedMovieAsync_ValidOwnership_CreatesOneRecord()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser("buyer", 100m);
            Movie movie = BuildMovie("Inception");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);
            await repository.AddOwnedMovieAsync(new OwnedMovie { User = user, Movie = movie });
            await repository.SaveChangesAsync();

            Assert.Single(context.OwnedMovies);
        }

        [Fact]
        public async Task AddTransactionAsync_ValidTransaction_CreatesOneRecord()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser("buyer", 100m);
            context.Users.Add(user);
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);
            await repository.AddTransactionAsync(new Transaction
            {
                Buyer = user,
                Amount = -10m,
                Type = "MoviePurchase",
                Status = "Completed",
                Timestamp = DateTime.UtcNow
            });
            await repository.SaveChangesAsync();

            Assert.Single(context.Transactions);
        }

        private static Movie BuildMovie(string title)
        {
            return new Movie
            {
                Title = title,
                Description = "desc",
                PrimaryGenre = "Drama",
                Synopsis = "synopsis",
                Price = 10m,
                Rating = 0m,
                ReleaseYear = 2000
            };
        }

        private static User BuildUser(string name, decimal balance)
        {
            return new User
            {
                Username = name,
                Email = $"{name}@example.com",
                PasswordHash = "hash",
                Balance = balance
            };
        }
    }
}
