using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class MovieRepositoryTests
    {
        [Fact]
        public void GetAllMovies_NoMovies_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            MovieRepository repository = new MovieRepository(context);

            List<Movie> result = repository.GetAllMovies();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllMovies_MultipleMovies_ReturnsAllOrderedByTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            context.Movies.AddRange(
                BuildMovie("Zodiac"),
                BuildMovie("Arrival"),
                BuildMovie("Inception"));
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);
            List<Movie> result = repository.GetAllMovies();

            Assert.Equal(3, result.Count);
            Assert.Equal("Arrival", result[0].Title);
            Assert.Equal("Inception", result[1].Title);
            Assert.Equal("Zodiac", result[2].Title);
        }

        [Fact]
        public void GetMovieById_ExistingId_ReturnsMovie()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie("Inception");
            context.Movies.Add(movie);
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);
            Movie? result = repository.GetMovieById(movie.Id);

            Assert.NotNull(result);
            Assert.Equal("Inception", result!.Title);
        }

        [Fact]
        public void GetMovieById_UnknownId_ReturnsNull()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            MovieRepository repository = new MovieRepository(context);

            Assert.Null(repository.GetMovieById(123));
        }

        [Fact]
        public void UserOwnsMovie_InvalidUserId_ReturnsFalse()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            MovieRepository repository = new MovieRepository(context);

            Assert.False(repository.UserOwnsMovie(0, 1));
            Assert.False(repository.UserOwnsMovie(-1, 1));
        }

        [Fact]
        public void UserOwnsMovie_OwnsMovie_ReturnsTrue()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser("buyer", 100m);
            Movie movie = BuildMovie("Inception");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.OwnedMovies.Add(new OwnedMovie { User = user, Movie = movie });
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);

            Assert.True(repository.UserOwnsMovie(user.Id, movie.Id));
        }

        [Fact]
        public void PurchaseMovie_InvalidUserId_ThrowsInvalidOperation()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            MovieRepository repository = new MovieRepository(context);

            Assert.Throws<InvalidOperationException>(() => repository.PurchaseMovie(0, 1, 9.99m));
        }

        [Fact]
        public void PurchaseMovie_HappyPath_DeductsBalanceAndCreatesOwnership()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser("buyer", 100m);
            Movie movie = BuildMovie("Inception");
            movie.Price = 30m;
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);
            repository.PurchaseMovie(user.Id, movie.Id, 30m);

            Assert.Equal(70m, context.Users.Single().Balance);
            Assert.Single(context.OwnedMovies);
            Assert.Single(context.Transactions);
        }

        [Fact]
        public void PurchaseMovie_AlreadyOwned_Throws()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser("buyer", 100m);
            Movie movie = BuildMovie("Inception");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.OwnedMovies.Add(new OwnedMovie { User = user, Movie = movie });
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);

            Assert.Throws<InvalidOperationException>(() => repository.PurchaseMovie(user.Id, movie.Id, 10m));
        }

        [Fact]
        public void PurchaseMovie_InsufficientBalance_Throws()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser("buyer", 5m);
            Movie movie = BuildMovie("Inception");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);

            Assert.Throws<InvalidOperationException>(() => repository.PurchaseMovie(user.Id, movie.Id, 10m));
        }

        [Fact]
        public async Task SearchTop10MoviesAsync_FiltersByPartialTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            context.Movies.AddRange(
                BuildMovie("The Matrix"),
                BuildMovie("The Matrix Reloaded"),
                BuildMovie("Inception"));
            context.SaveChanges();

            MovieRepository repository = new MovieRepository(context);
            List<Movie> result = await repository.SearchTop10MoviesAsync("Matrix");

            Assert.Equal(2, result.Count);
            Assert.All(result, movie => Assert.Contains("Matrix", movie.Title));
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
