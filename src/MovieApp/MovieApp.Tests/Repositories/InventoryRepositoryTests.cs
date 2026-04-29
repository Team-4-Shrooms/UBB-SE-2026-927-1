using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class InventoryRepositoryTests
    {
        [Fact]
        public async Task GetOwnedMoviesAsync_UserOwnsTwoMovies_ReturnsTwoMovies()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movieA = BuildMovie("MovieA");
            Movie movieB = BuildMovie("MovieB");
            context.Users.Add(user);
            context.Movies.AddRange(movieA, movieB);
            context.OwnedMovies.AddRange(
                new OwnedMovie { User = user, Movie = movieA },
                new OwnedMovie { User = user, Movie = movieB });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            List<Movie> ownedMovies = await repository.GetOwnedMoviesAsync(user.Id);

            Assert.Equal(2, ownedMovies.Count);
        }

        [Fact]
        public async Task GetMovieOwnershipsAsync_OwnershipExists_ReturnsSingleOwnership()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("TestMovie");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.OwnedMovies.Add(new OwnedMovie { User = user, Movie = movie });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            List<OwnedMovie> ownerships = await repository.GetMovieOwnershipsAsync(user.Id, movie.Id);

            Assert.Single(ownerships);
        }

        [Fact]
        public async Task GetMovieOwnershipsAsync_OwnershipDoesNotExist_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("TestMovie");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            List<OwnedMovie> ownerships = await repository.GetMovieOwnershipsAsync(user.Id, movie.Id);

            Assert.Empty(ownerships);
        }

        [Fact]
        public void RemoveMovieOwnerships_OneOwnershipProvided_RemovesOwnershipFromDatabase()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("TestMovie");
            context.Users.Add(user);
            context.Movies.Add(movie);
            OwnedMovie ownership = new OwnedMovie { User = user, Movie = movie };
            context.OwnedMovies.Add(ownership);
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            repository.RemoveMovieOwnerships(new[] { ownership });
            context.SaveChanges();

            Assert.Empty(context.OwnedMovies);
        }

        [Fact]
        public async Task GetTicketOwnershipsAsync_OwnershipExists_ReturnsSingleOwnership()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("TestMovie");
            MovieEvent movieEvent = BuildEvent(movie);
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.OwnedTickets.Add(new OwnedTicket { User = user, Event = movieEvent });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            List<OwnedTicket> ownerships = await repository.GetTicketOwnershipsAsync(user.Id, movieEvent.Id);

            Assert.Single(ownerships);
        }

        [Fact]
        public async Task GetTicketOwnershipsAsync_OwnershipDoesNotExist_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("TestMovie");
            MovieEvent movieEvent = BuildEvent(movie);
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            List<OwnedTicket> ownerships = await repository.GetTicketOwnershipsAsync(user.Id, movieEvent.Id);

            Assert.Empty(ownerships);
        }

        [Fact]
        public void RemoveTicketOwnerships_OneOwnershipProvided_RemovesOwnershipFromDatabase()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("TestMovie");
            MovieEvent movieEvent = BuildEvent(movie);
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            OwnedTicket ownership = new OwnedTicket { User = user, Event = movieEvent };
            context.OwnedTickets.Add(ownership);
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            repository.RemoveTicketOwnerships(new[] { ownership });
            context.SaveChanges();

            Assert.Empty(context.OwnedTickets);
        }

        [Fact]
        public async Task AddTransactionAsync_ValidTransaction_CreatesOneRecord()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            context.Users.Add(user);
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            await repository.AddTransactionAsync(new Transaction
            {
                Buyer = user,
                Amount = -10m,
                Type = "RemoveOwnedMovie",
                Status = "Completed",
                Timestamp = DateTime.UtcNow
            });
            await repository.SaveChangesAsync();

            Assert.Single(context.Transactions);
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
                ReleaseYear = 2010
            };
        }

        private static MovieEvent BuildEvent(Movie movie)
        {
            return new MovieEvent
            {
                Title = "Show",
                Description = "desc",
                Date = DateTime.UtcNow.AddDays(1),
                Location = "Cinema",
                TicketPrice = 10m,
                PosterUrl = "https://example.com/poster.jpg",
                Movie = movie
            };
        }
    }
}
