using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;

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
        public void GetAllEvents_ReturnsOrderedByDate()
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
            Assert.Equal("Early", result[0].Title);
            Assert.Equal("Late", result[1].Title);
        }

        [Fact]
        public void GetEventsForMovie_FiltersByMovieId()
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
            Assert.Equal("A1", result[0].Title);
        }

        [Fact]
        public void GetEventById_ExistingId_ReturnsEvent()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            MovieEvent movieEvent = BuildEvent(movie, "Show", DateTime.UtcNow.AddDays(1));
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            MovieEvent? result = repository.GetEventById(movieEvent.Id);

            Assert.NotNull(result);
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
        public void PurchaseTicket_InvalidUserId_Throws()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EventRepository repository = new EventRepository(context);

            Assert.Throws<InvalidOperationException>(() => repository.PurchaseTicket(0, 1));
            Assert.Throws<InvalidOperationException>(() => repository.PurchaseTicket(-1, 1));
        }

        [Fact]
        public void PurchaseTicket_HappyPath_DeductsBalanceAndCreatesOwnership()
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
            Assert.Single(context.OwnedTickets);
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
