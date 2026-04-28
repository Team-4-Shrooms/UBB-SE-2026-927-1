using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class EventRepositoryTests
    {
        [Fact]
        public void GetAllEvents_NoEvents_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EventRepository repository = new EventRepository(context);

            Assert.Empty(repository.GetAllEvents());
        }

        [Fact]
        public void GetAllEvents_ReturnsCorrectCount()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            context.Movies.Add(movie);
            context.MovieEvents.AddRange(
                BuildEvent(movie, "Late", DateTime.UtcNow.AddDays(10)),
                BuildEvent(movie, "Early", DateTime.UtcNow.AddDays(1)));
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            List<MovieEvent> result = repository.GetAllEvents();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllEvents_OrdersByDateAscending()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            context.Movies.Add(movie);
            context.MovieEvents.AddRange(
                BuildEvent(movie, "Late", DateTime.UtcNow.AddDays(10)),
                BuildEvent(movie, "Early", DateTime.UtcNow.AddDays(1)));
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            List<string> titles = repository.GetAllEvents()
                .Select(movieEvent => movieEvent.Title)
                .ToList();

            Assert.Equal(new[] { "Early", "Late" }, titles);
        }

        [Fact]
        public void GetEventsForMovie_ReturnsOnlyEventsForRequestedMovie()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movieA = BuildMovie();
            Movie movieB = BuildMovie();
            context.Movies.AddRange(movieA, movieB);
            context.MovieEvents.AddRange(
                BuildEvent(movieA, "A1", DateTime.UtcNow.AddDays(1)),
                BuildEvent(movieB, "B1", DateTime.UtcNow.AddDays(2)));
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            List<MovieEvent> result = repository.GetEventsForMovie(movieA.Id);

            Assert.Single(result);
        }

        [Fact]
        public void GetEventsForMovie_ResultMatchesRequestedMovieTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movieA = BuildMovie();
            Movie movieB = BuildMovie();
            context.Movies.AddRange(movieA, movieB);
            context.MovieEvents.AddRange(
                BuildEvent(movieA, "A1", DateTime.UtcNow.AddDays(1)),
                BuildEvent(movieB, "B1", DateTime.UtcNow.AddDays(2)));
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            List<MovieEvent> result = repository.GetEventsForMovie(movieA.Id);

            Assert.Equal("A1", result[0].Title);
        }

        [Fact]
        public void GetEventById_ExistingId_ReturnsEventWithMatchingTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            MovieEvent movieEvent = BuildEvent(movie, "Show", DateTime.UtcNow.AddDays(1));
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            MovieEvent? result = repository.GetEventById(movieEvent.Id);

            Assert.Equal("Show", result!.Title);
        }

        [Fact]
        public void GetEventById_UnknownId_ReturnsNull()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EventRepository repository = new EventRepository(context);

            Assert.Null(repository.GetEventById(99));
        }

        [Fact]
        public void PurchaseTicket_ZeroUserId_Throws()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EventRepository repository = new EventRepository(context);

            Assert.Throws<InvalidOperationException>(() => repository.PurchaseTicket(0, 1));
        }

        [Fact]
        public void PurchaseTicket_NegativeUserId_Throws()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EventRepository repository = new EventRepository(context);

            Assert.Throws<InvalidOperationException>(() => repository.PurchaseTicket(-1, 1));
        }

        [Fact]
        public void PurchaseTicket_HappyPath_DeductsBalance()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser(100m);
            Movie movie = BuildMovie();
            MovieEvent movieEvent = BuildEvent(movie, "Show", DateTime.UtcNow.AddDays(1));
            movieEvent.TicketPrice = 25m;
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            repository.PurchaseTicket(user.Id, movieEvent.Id);

            Assert.Equal(75m, context.Users.Single().Balance);
        }

        [Fact]
        public void PurchaseTicket_HappyPath_CreatesOwnership()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser(100m);
            Movie movie = BuildMovie();
            MovieEvent movieEvent = BuildEvent(movie, "Show", DateTime.UtcNow.AddDays(1));
            movieEvent.TicketPrice = 25m;
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            repository.PurchaseTicket(user.Id, movieEvent.Id);

            Assert.Single(context.OwnedTickets);
        }

        [Fact]
        public void PurchaseTicket_HappyPath_LogsTransaction()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser(100m);
            Movie movie = BuildMovie();
            MovieEvent movieEvent = BuildEvent(movie, "Show", DateTime.UtcNow.AddDays(1));
            movieEvent.TicketPrice = 25m;
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            repository.PurchaseTicket(user.Id, movieEvent.Id);

            Assert.Single(context.Transactions);
        }

        [Fact]
        public void PurchaseTicket_AlreadyOwned_Throws()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser(100m);
            Movie movie = BuildMovie();
            MovieEvent movieEvent = BuildEvent(movie, "Show", DateTime.UtcNow.AddDays(1));
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.OwnedTickets.Add(new OwnedTicket { User = user, Event = movieEvent });
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);

            Assert.Throws<InvalidOperationException>(() => repository.PurchaseTicket(user.Id, movieEvent.Id));
        }

        [Fact]
        public void PurchaseTicket_InsufficientBalance_Throws()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser(5m);
            Movie movie = BuildMovie();
            MovieEvent movieEvent = BuildEvent(movie, "Show", DateTime.UtcNow.AddDays(1));
            movieEvent.TicketPrice = 25m;
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);

            Assert.Throws<InvalidOperationException>(() => repository.PurchaseTicket(user.Id, movieEvent.Id));
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

        private static MovieEvent BuildEvent(Movie movie, string title, DateTime date)
        {
            return new MovieEvent
            {
                Title = title,
                Description = "desc",
                Date = date,
                Location = "Cinema",
                TicketPrice = 10m,
                PosterUrl = "https://example.com/poster.jpg",
                Movie = movie
            };
        }

        private static User BuildUser(decimal balance)
        {
            return new User
            {
                Username = "buyer",
                Email = "buyer@example.com",
                PasswordHash = "hash",
                Balance = balance
            };
        }
    }
}
