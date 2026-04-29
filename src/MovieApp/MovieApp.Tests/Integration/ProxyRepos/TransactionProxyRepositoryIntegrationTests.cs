using MovieApp.DataLayer.Models;
using MovieApp.Logic.Http;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class TransactionProxyRepositoryIntegrationTests
{
    [Fact]
    public void LogTransaction_ExistingBuyerAndMovie_PersistsTransactionForBuyer()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        TransactionProxyRepository transactionRepository = new TransactionProxyRepository(testContext.ApiClient);

        transactionRepository.LogTransaction(new Transaction
        {
            Amount = 19.99m,
            Type = "BuyMovie",
            Status = "Completed",
            Timestamp = DateTime.UtcNow,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
        });

        List<Transaction> transactions = transactionRepository.GetTransactionsByUserId(ProxyRepoSeedIds.SeededUserId);

        Assert.True(transactions.Any(transaction => transaction.Type == "BuyMovie"));
    }

    [Fact]
    public void UpdateTransactionStatus_ExistingTransaction_ChangesStatus()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EventProxyRepository eventRepository = new EventProxyRepository(testContext.ApiClient);
        TransactionProxyRepository transactionRepository = new TransactionProxyRepository(testContext.ApiClient);
        List<MovieEvent> allEvents = eventRepository.GetAllEventsAsync().GetAwaiter().GetResult();
        int eventId = allEvents[0].Id;

        transactionRepository.LogTransaction(new Transaction
        {
            Amount = 8.50m,
            Type = "BuyTicket",
            Status = "Pending",
            Timestamp = DateTime.UtcNow,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Event = new MovieEvent { Id = eventId },
        });

        List<Transaction> transactions = transactionRepository.GetTransactionsByUserId(ProxyRepoSeedIds.SeededUserId);
        int transactionId = transactions[0].Id;
        transactionRepository.UpdateTransactionStatus(transactionId, "Completed");

        List<Transaction> updatedTransactions = transactionRepository.GetTransactionsByUserId(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal("Completed", updatedTransactions[0].Status);
    }
}
