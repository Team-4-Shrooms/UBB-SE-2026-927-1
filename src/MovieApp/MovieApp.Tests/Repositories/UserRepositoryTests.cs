using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class UserRepositoryTests
    {
        private const int UnknownUserId = 999;

        [Fact]
        public void GetBalance_ExistingUser_ReturnsBalance()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = new User
            {
                Username = "Alice",
                Email = "alice@example.com",
                PasswordHash = "hash",
                Balance = 250.50m
            };
            context.Users.Add(user);
            context.SaveChanges();

            UserRepository repository = new UserRepository(context);
            decimal balance = repository.GetBalance(user.Id);

            Assert.Equal(250.50m, balance);
        }

        [Fact]
        public void GetBalance_ZeroUserId_ReturnsZero()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            UserRepository repository = new UserRepository(context);

            Assert.Equal(0m, repository.GetBalance(0));
        }

        [Fact]
        public void GetBalance_NegativeUserId_ReturnsZero()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            UserRepository repository = new UserRepository(context);

            Assert.Equal(0m, repository.GetBalance(-5));
        }

        [Fact]
        public void GetBalance_UnknownUser_ReturnsZero()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            UserRepository repository = new UserRepository(context);

            Assert.Equal(0m, repository.GetBalance(UnknownUserId));
        }

        [Fact]
        public void UpdateBalance_ExistingUser_UpdatesBalance()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = new User
            {
                Username = "Bob",
                Email = "bob@example.com",
                PasswordHash = "hash",
                Balance = 100m
            };
            context.Users.Add(user);
            context.SaveChanges();

            UserRepository repository = new UserRepository(context);
            repository.UpdateBalance(user.Id, 999.99m);

            Assert.Equal(999.99m, context.Users.Single().Balance);
        }

        [Fact]
        public void UpdateBalance_InvalidUserId_DoesNotThrow()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            UserRepository repository = new UserRepository(context);

            Exception? exception = Record.Exception(() => repository.UpdateBalance(0, 100m));
            Assert.Null(exception);
        }
    }
}
