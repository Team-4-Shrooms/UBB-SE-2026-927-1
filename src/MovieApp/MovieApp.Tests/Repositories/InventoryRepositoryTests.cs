using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class InventoryRepositoryTests
    {
        [Fact]
        public void GetOwnedMovies_ReturnsMoviesOwnedByUser()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movieA = BuildMovie("A");
            Movie movieB = BuildMovie("B");
            context.Users.Add(user);
            context.Movies.AddRange(movieA, movieB);
            context.OwnedMovies.AddRange(
                new OwnedMovie { User = user, Movie = movieA },
                new OwnedMovie { User = user, Movie = movieB });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            List<Movie> owned = repository.GetOwnedMovies(user.Id);

            Assert.Equal(2, owned.Count);
        }

        [Fact]
        public void GetOwnedTickets_ReturnsTicketsOwnedByUser()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("M");
            MovieEvent movieEvent = BuildEvent(movie);
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.OwnedTickets.Add(new OwnedTicket { User = user, Event = movieEvent });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            List<MovieEvent> tickets = repository.GetOwnedTickets(user.Id);

            Assert.Single(tickets);
        }

        [Fact]
        public void GetOwnedEquipment_ReturnsEquipmentBoughtByUser_ReturnsSingleItem()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User buyer = BuildUser();
            User seller = BuildUser();
            seller.Username = "seller";
            seller.Email = "seller@example.com";
            Equipment equipment = BuildEquipment(seller);
            context.Users.AddRange(buyer, seller);
            context.Equipment.Add(equipment);
            context.Transactions.Add(BuildEquipmentPurchase(buyer, seller, equipment));
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            List<Equipment> result = repository.GetOwnedEquipment(buyer.Id);

            Assert.Single(result);
        }

        [Fact]
        public void GetOwnedEquipment_ReturnsEquipmentBoughtByUser_ResultHasMatchingTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User buyer = BuildUser();
            User seller = BuildUser();
            seller.Username = "seller";
            seller.Email = "seller@example.com";
            Equipment equipment = BuildEquipment(seller);
            context.Users.AddRange(buyer, seller);
            context.Equipment.Add(equipment);
            context.Transactions.Add(BuildEquipmentPurchase(buyer, seller, equipment));
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            List<Equipment> result = repository.GetOwnedEquipment(buyer.Id);

            Assert.Equal("Camera", result[0].Title);
        }

        [Fact]
        public void RemoveOwnedMovie_RemovesOwnership()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("M");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.OwnedMovies.Add(new OwnedMovie { User = user, Movie = movie });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            repository.RemoveOwnedMovie(user.Id, movie.Id);

            Assert.Empty(context.OwnedMovies);
        }

        [Fact]
        public void RemoveOwnedMovie_LogsSingleTransaction()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("M");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.OwnedMovies.Add(new OwnedMovie { User = user, Movie = movie });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            repository.RemoveOwnedMovie(user.Id, movie.Id);

            Assert.Single(context.Transactions);
        }

        [Fact]
        public void RemoveOwnedMovie_LogsTransactionWithRemoveType()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("M");
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.OwnedMovies.Add(new OwnedMovie { User = user, Movie = movie });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            repository.RemoveOwnedMovie(user.Id, movie.Id);

            Assert.Equal("RemoveOwnedMovie", context.Transactions.Single().Type);
        }

        [Fact]
        public void RemoveOwnedMovie_InvalidUserId_DoesNotThrow()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            InventoryRepository repository = new InventoryRepository(context);

            Exception? exception = Record.Exception(() => repository.RemoveOwnedMovie(0, 1));

            Assert.Null(exception);
        }

        [Fact]
        public void RemoveOwnedTicket_RemovesOwnership()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("M");
            MovieEvent movieEvent = BuildEvent(movie);
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.OwnedTickets.Add(new OwnedTicket { User = user, Event = movieEvent });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            repository.RemoveOwnedTicket(user.Id, movieEvent.Id);

            Assert.Empty(context.OwnedTickets);
        }

        [Fact]
        public void RemoveOwnedTicket_LogsSingleTransaction()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("M");
            MovieEvent movieEvent = BuildEvent(movie);
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.OwnedTickets.Add(new OwnedTicket { User = user, Event = movieEvent });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            repository.RemoveOwnedTicket(user.Id, movieEvent.Id);

            Assert.Single(context.Transactions);
        }

        [Fact]
        public void RemoveOwnedTicket_LogsTransactionWithRemoveType()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser();
            Movie movie = BuildMovie("M");
            MovieEvent movieEvent = BuildEvent(movie);
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.OwnedTickets.Add(new OwnedTicket { User = user, Event = movieEvent });
            context.SaveChanges();

            InventoryRepository repository = new InventoryRepository(context);
            repository.RemoveOwnedTicket(user.Id, movieEvent.Id);

            Assert.Equal("RemoveOwnedTicket", context.Transactions.Single().Type);
        }

        [Fact]
        public void RemoveOwnedTicket_InvalidUserId_DoesNotThrow()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            InventoryRepository repository = new InventoryRepository(context);

            Exception? exception = Record.Exception(() => repository.RemoveOwnedTicket(-1, 1));

            Assert.Null(exception);
        }

        private static Equipment BuildEquipment(User seller)
        {
            return new Equipment
            {
                Title = "Camera",
                Category = "Cameras",
                Description = "desc",
                Condition = "New",
                Price = 100m,
                ImageUrl = "https://example.com/camera.jpg",
                Status = EquipmentStatus.Sold,
                Seller = seller
            };
        }

        private static Transaction BuildEquipmentPurchase(User buyer, User seller, Equipment equipment)
        {
            return new Transaction
            {
                Buyer = buyer,
                Seller = seller,
                Equipment = equipment,
                Amount = -100m,
                Type = "EquipmentPurchase",
                Status = "Completed",
                Timestamp = DateTime.UtcNow
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
