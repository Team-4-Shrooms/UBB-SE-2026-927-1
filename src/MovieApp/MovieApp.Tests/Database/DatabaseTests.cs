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

        [Fact]
        public void AddMovie_SavesToDatabaseSuccessfully()
        {
            // Arrange
            Movie movie = new Movie
            {
                Title = "Inception",
                Description = "A mind-bending thriller.",
                Price = 14.99m,
                Rating = 8.8,
                PrimaryGenre = "Sci-Fi",
                ReleaseYear = 2010
            };

            // Act
            _context.Movies.Add(movie);
            _context.SaveChanges();

            // Assert
            int movieCount = _context.Movies.Count();
            Assert.Equal(1, movieCount);

            Movie savedMovie = _context.Movies.First();
            Assert.Equal("Inception", savedMovie.Title);
            Assert.Equal(14.99m, savedMovie.Price);
        }

        [Fact]
        public void AddUser_WithOwnedMovie_SavesRelationshipsSuccessfully()
        {
            // Arrange
            Movie movie = new Movie
            {
                Title = "The Matrix",
                Description = "A computer hacker learns from mysterious rebels about the true nature of his reality.", // <-- Add this line!
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

            // Act
            _context.Users.Add(user);
            _context.SaveChanges();

            // Assert
            User retrievedUser = _context.Users
                .Include(u => u.OwnedMovies)
                .ThenInclude(om => om.Movie)
                .First(u => u.Username == "TestUser");

            int ownedCount = retrievedUser.OwnedMovies.Count;
            Assert.Equal(1, ownedCount);

            string retrievedMovieTitle = retrievedUser.OwnedMovies.First().Movie.Title;
            Assert.Equal("The Matrix", retrievedMovieTitle);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}