using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class EventRepositoryTests
    {
        [Fact]
        public async Task GetAllEventsAsync_NoEventsExist_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EventRepository repository = new EventRepository(context);

            List<MovieEvent> allEvents = await repository.GetAllEventsAsync();

            Assert.Empty(allEvents);
        }

        [Fact]
        public async Task GetAllEventsAsync_TwoEventsExist_ReturnsTwoEvents()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            context.Movies.Add(movie);
            context.MovieEvents.AddRange(
                BuildEvent(movie, "Late", DateTime.UtcNow.AddDays(10)),
                BuildEvent(movie, "Early", DateTime.UtcNow.AddDays(1)));
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            List<MovieEvent> allEvents = await repository.GetAllEventsAsync();

            Assert.Equal(2, allEvents.Count);
        }

        [Fact]
        public async Task GetAllEventsAsync_TwoEventsExist_ReturnsEventsOrderedByDateAscending()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            context.Movies.Add(movie);
            context.MovieEvents.AddRange(
                BuildEvent(movie, "Late", DateTime.UtcNow.AddDays(10)),
                BuildEvent(movie, "Early", DateTime.UtcNow.AddDays(1)));
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            List<string> eventTitles = (await repository.GetAllEventsAsync())
                .Select(movieEvent => movieEvent.Title)
                .ToList();

            Assert.Equal(new[] { "Early", "Late" }, eventTitles);
        }

        [Fact]
        public async Task GetEventsByMovieIdAsync_TwoMoviesWithEvents_ReturnsOnlyEventsForRequestedMovie()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movieA = BuildMovie();
            Movie movieB = BuildMovie();
            context.Movies.AddRange(movieA, movieB);
            context.MovieEvents.AddRange(
                BuildEvent(movieA, "EventA", DateTime.UtcNow.AddDays(1)),
                BuildEvent(movieB, "EventB", DateTime.UtcNow.AddDays(2)));
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            List<MovieEvent> eventsForMovieA = await repository.GetEventsByMovieIdAsync(movieA.Id);

            Assert.Single(eventsForMovieA);
        }

        [Fact]
        public async Task GetEventsByMovieIdAsync_TwoMoviesWithEvents_ReturnedEventHasCorrectTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movieA = BuildMovie();
            Movie movieB = BuildMovie();
            context.Movies.AddRange(movieA, movieB);
            context.MovieEvents.AddRange(
                BuildEvent(movieA, "EventA", DateTime.UtcNow.AddDays(1)),
                BuildEvent(movieB, "EventB", DateTime.UtcNow.AddDays(2)));
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            List<MovieEvent> eventsForMovieA = await repository.GetEventsByMovieIdAsync(movieA.Id);

            Assert.Equal("EventA", eventsForMovieA[0].Title);
        }

        [Fact]
        public async Task GetEventByIdAsync_EventExists_ReturnsEventWithMatchingTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            Movie movie = BuildMovie();
            MovieEvent movieEvent = BuildEvent(movie, "Show", DateTime.UtcNow.AddDays(1));
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            MovieEvent? foundEvent = await repository.GetEventByIdAsync(movieEvent.Id);

            Assert.Equal("Show", foundEvent!.Title);
        }

        [Fact]
        public async Task GetEventByIdAsync_EventDoesNotExist_ReturnsNull()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EventRepository repository = new EventRepository(context);

            MovieEvent? foundEvent = await repository.GetEventByIdAsync(99);

            Assert.Null(foundEvent);
        }

        [Fact]
        public async Task UserHasTicketAsync_UserOwnsTicket_ReturnsTrue()
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

            bool hasTicket = await repository.UserHasTicketAsync(user.Id, movieEvent.Id);

            Assert.True(hasTicket);
        }

        [Fact]
        public async Task UserHasTicketAsync_UserDoesNotOwnTicket_ReturnsFalse()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser(100m);
            Movie movie = BuildMovie();
            MovieEvent movieEvent = BuildEvent(movie, "Show", DateTime.UtcNow.AddDays(1));
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);

            bool hasTicket = await repository.UserHasTicketAsync(user.Id, movieEvent.Id);

            Assert.False(hasTicket);
        }

        [Fact]
        public async Task AddOwnedTicketAsync_ValidTicket_CreatesOneRecord()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildUser(100m);
            Movie movie = BuildMovie();
            MovieEvent movieEvent = BuildEvent(movie, "Show", DateTime.UtcNow.AddDays(1));
            context.Users.Add(user);
            context.Movies.Add(movie);
            context.MovieEvents.Add(movieEvent);
            context.SaveChanges();

            EventRepository repository = new EventRepository(context);
            await repository.AddOwnedTicketAsync(new OwnedTicket { User = user, Event = movieEvent });
            await repository.SaveChangesAsync();

            Assert.Single(context.OwnedTickets);
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
