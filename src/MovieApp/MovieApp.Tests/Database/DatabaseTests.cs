using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using Xunit;

namespace MovieApp.Tests.Database
{
    public class DatabaseTests : IDisposable
    {
        private readonly AppDbContext _context;

        public DatabaseTests()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
        }

        // --- ADD TESTS ---

        [Fact]
        public void Add_ValidMovie_IncreasesMovieCount()
        {
            Movie movie = new Movie
            {
                Title = "Inception",
                Description = "A mind-bending thriller.",
                Price = 14.99m,
                Rating = 8.8m,
                PrimaryGenre = "Sci-Fi",
                ReleaseYear = 2010
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();

            int movieCount = _context.Movies.Count();
            Assert.Equal(1, movieCount);
        }

        [Fact]
        public void Add_ValidMovie_SavesCorrectTitle()
        {
            Movie movie = new Movie
            {
                Title = "Inception",
                Description = "A mind-bending thriller.",
                Price = 14.99m,
                Rating = 8.8m,
                PrimaryGenre = "Sci-Fi",
                ReleaseYear = 2010
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();

            Movie savedMovie = _context.Movies.First();
            Assert.Equal("Inception", savedMovie.Title);
        }

        [Fact]
        public void Add_ValidMovie_SavesCorrectPrice()
        {
            Movie movie = new Movie
            {
                Title = "Inception",
                Description = "A mind-bending thriller.",
                Price = 14.99m,
                Rating = 8.8m,
                PrimaryGenre = "Sci-Fi",
                ReleaseYear = 2010
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();

            Movie savedMovie = _context.Movies.First();
            Assert.Equal(14.99m, savedMovie.Price);
        }

        [Fact]
        public void Add_UserWithOwnedMovie_SavesRelationshipCount()
        {
            Movie movie = new Movie
            {
                Title = "The Matrix",
                Description = "A computer hacker learns from mysterious rebels.",
                Price = 9.99m,
                PrimaryGenre = "Action"
            };

            User user = new User
            {
                Username = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                Balance = 50.0m
            };

            OwnedMovie ownership = new OwnedMovie
            {
                User = user,
                Movie = movie
            };

            user.OwnedMovies.Add(ownership);

            _context.Users.Add(user);
            _context.SaveChanges();

            User retrievedUser = _context.Users
                .Include(u => u.OwnedMovies)
                .ThenInclude(om => om.Movie)
                .First(u => u.Username == "TestUser");

            int ownedCount = retrievedUser.OwnedMovies.Count;
            Assert.Equal(1, ownedCount);
        }

        [Fact]
        public void Add_UserWithOwnedMovie_SavesCorrectRelatedMovieTitle()
        {
            Movie movie = new Movie
            {
                Title = "The Matrix",
                Description = "A computer hacker learns from mysterious rebels.",
                Price = 9.99m,
                PrimaryGenre = "Action"
            };

            User user = new User
            {
                Username = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                Balance = 50.0m
            };

            OwnedMovie ownership = new OwnedMovie
            {
                User = user,
                Movie = movie
            };

            user.OwnedMovies.Add(ownership);

            _context.Users.Add(user);
            _context.SaveChanges();

            User retrievedUser = _context.Users
                .Include(u => u.OwnedMovies)
                .ThenInclude(om => om.Movie)
                .First(u => u.Username == "TestUser");

            string retrievedMovieTitle = retrievedUser.OwnedMovies.First().Movie.Title;
            Assert.Equal("The Matrix", retrievedMovieTitle);
        }

        // --- UPDATE TESTS ---

        [Fact]
        public void Update_ExistingMovie_SavesModifiedProperty()
        {
            Movie movie = new Movie
            {
                Title = "Inception",
                Description = "A mind-bending thriller.",
                Price = 14.99m,
                Rating = 8.8m,
                PrimaryGenre = "Sci-Fi",
                ReleaseYear = 2010
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();

            Movie savedMovie = _context.Movies.First();
            savedMovie.Price = 9.99m;
            _context.SaveChanges();

            Movie updatedMovie = _context.Movies.First();
            Assert.Equal(9.99m, updatedMovie.Price);
        }

        // --- REMOVE TESTS ---

        [Fact]
        public void Remove_ExistingMovie_DecreasesMovieCount()
        {
            Movie movie = new Movie
            {
                Title = "Inception",
                Description = "A mind-bending thriller.",
                Price = 14.99m,
                Rating = 8.8m,
                PrimaryGenre = "Sci-Fi",
                ReleaseYear = 2010
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();

            Movie savedMovie = _context.Movies.First();
            _context.Movies.Remove(savedMovie);
            _context.SaveChanges();

            int movieCount = _context.Movies.Count();
            Assert.Equal(0, movieCount);
        }

        // --- DISPOSE ---

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}