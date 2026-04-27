using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class TransactionRepositoryTests
    {
        [Fact]
        public void LogTransaction_AddsTransaction()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User buyer = BuildUser("buyer");
            context.Users.Add(buyer);
            context.SaveChanges();

            TransactionRepository repository = new TransactionRepository(context);
            Transaction transaction = new Transaction
            {
                Buyer = buyer,
                Amount = 50m,
                Type = "Test",
                Status = "Completed",
                Timestamp = DateTime.UtcNow
            };
            repository.LogTransaction(transaction);

            Assert.Single(context.Transactions);
        }

        [Fact]
        public void GetTransactionsByUserId_ReturnsBuyerTransactions()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User buyer = BuildUser("buyer");
            User seller = BuildUser("seller");
            User stranger = BuildUser("stranger");
            context.Users.AddRange(buyer, seller, stranger);
            context.Transactions.AddRange(
                BuildTransaction(buyer, null, DateTime.UtcNow.AddMinutes(-10)),
                BuildTransaction(stranger, seller, DateTime.UtcNow.AddMinutes(-5)),
                BuildTransaction(stranger, null, DateTime.UtcNow));
            context.SaveChanges();

            TransactionRepository repository = new TransactionRepository(context);
            List<Transaction> buyerResults = repository.GetTransactionsByUserId(buyer.Id);

            Assert.Single(buyerResults);
        }

        [Fact]
        public void GetTransactionsByUserId_ReturnsSellerTransactions()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User buyer = BuildUser("buyer");
            User seller = BuildUser("seller");
            User stranger = BuildUser("stranger");
            context.Users.AddRange(buyer, seller, stranger);
            context.Transactions.AddRange(
                BuildTransaction(buyer, null, DateTime.UtcNow.AddMinutes(-10)),
                BuildTransaction(stranger, seller, DateTime.UtcNow.AddMinutes(-5)),
                BuildTransaction(stranger, null, DateTime.UtcNow));
            context.SaveChanges();

            TransactionRepository repository = new TransactionRepository(context);
            List<Transaction> sellerResults = repository.GetTransactionsByUserId(seller.Id);

            Assert.Single(sellerResults);
        }

        [Fact]
        public void GetTransactionsByUserId_ReturnsCorrectCount()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User buyer = BuildUser("buyer");
            context.Users.Add(buyer);
            DateTime now = DateTime.UtcNow;
            context.Transactions.AddRange(
                BuildTransaction(buyer, null, now.AddMinutes(-30)),
                BuildTransaction(buyer, null, now.AddMinutes(-1)),
                BuildTransaction(buyer, null, now.AddMinutes(-10)));
            context.SaveChanges();

            TransactionRepository repository = new TransactionRepository(context);
            List<Transaction> result = repository.GetTransactionsByUserId(buyer.Id);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void GetTransactionsByUserId_OrdersByTimestampDescending()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User buyer = BuildUser("buyer");
            context.Users.Add(buyer);
            DateTime now = DateTime.UtcNow;
            context.Transactions.AddRange(
                BuildTransaction(buyer, null, now.AddMinutes(-30)),
                BuildTransaction(buyer, null, now.AddMinutes(-1)),
                BuildTransaction(buyer, null, now.AddMinutes(-10)));
            context.SaveChanges();

            TransactionRepository repository = new TransactionRepository(context);
            List<Transaction> result = repository.GetTransactionsByUserId(buyer.Id);

            bool isDescending = result[0].Timestamp >= result[1].Timestamp
                && result[1].Timestamp >= result[2].Timestamp;

            Assert.True(isDescending);
        }

        [Fact]
        public void UpdateTransactionStatus_ExistingTransaction_UpdatesStatus()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User buyer = BuildUser("buyer");
            context.Users.Add(buyer);
            Transaction transaction = BuildTransaction(buyer, null, DateTime.UtcNow);
            context.Transactions.Add(transaction);
            context.SaveChanges();

            TransactionRepository repository = new TransactionRepository(context);
            repository.UpdateTransactionStatus(transaction.Id, "Refunded");

            Assert.Equal("Refunded", context.Transactions.Single().Status);
        }

        [Fact]
        public void UpdateTransactionStatus_UnknownTransaction_DoesNotThrow()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            TransactionRepository repository = new TransactionRepository(context);

            Exception? exception = Record.Exception(() => repository.UpdateTransactionStatus(999, "Refunded"));
            Assert.Null(exception);
        }

        private static User BuildUser(string name)
        {
            return new User
            {
                Username = name,
                Email = $"{name}@example.com",
                PasswordHash = "hash",
                Balance = 100m
            };
        }

        private static Transaction BuildTransaction(User buyer, User? seller, DateTime timestamp)
        {
            return new Transaction
            {
                Buyer = buyer,
                Seller = seller,
                Amount = 10m,
                Type = "Test",
                Status = "Completed",
                Timestamp = timestamp
            };
        }
    }
}
