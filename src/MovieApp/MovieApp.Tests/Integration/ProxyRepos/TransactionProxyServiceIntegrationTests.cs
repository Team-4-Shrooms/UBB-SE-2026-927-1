using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class TransactionProxyServiceIntegrationTests
{
    [Fact]
    public async Task LogTransaction_ExistingBuyerAndMovie_PersistsTransactionForBuyer()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        TransactionProxyService transactionRepository = new TransactionProxyService(testContext.ApiClient);

        await transactionRepository.LogTransactionAsync(new Transaction
        {
            Amount = 19.99m,
            Type = "BuyMovie",
            Status = "Completed",
            Timestamp = DateTime.UtcNow,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Movie = new Movie { Id = ProxyRepoSeedIds.SeededMovieId },
        });

        List<Transaction> transactions = await transactionRepository.GetTransactionsByUserIdAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.True(transactions.Any(transaction => transaction.Type == "BuyMovie"));
    }

    [Fact]
    public async Task UpdateTransactionStatus_ExistingTransaction_ChangesStatus()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        EventProxyService eventRepository = new EventProxyService(testContext.ApiClient);
        TransactionProxyService transactionRepository = new TransactionProxyService(testContext.ApiClient);
        List<MovieEvent> allEvents = await eventRepository.GetAvailableEventsAsync();
        int eventId = allEvents[0].Id;

        await transactionRepository.LogTransactionAsync(new Transaction
        {
            Amount = 8.50m,
            Type = "BuyTicket",
            Status = "Pending",
            Timestamp = DateTime.UtcNow,
            Buyer = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Event = new MovieEvent { Id = eventId },
        });

        List<Transaction> transactions = await transactionRepository.GetTransactionsByUserIdAsync(ProxyRepoSeedIds.SeededUserId);
        int transactionId = transactions[0].Id;
        await transactionRepository.UpdateTransactionStatusAsync(transactionId, "Completed");

        List<Transaction> updatedTransactions = await transactionRepository.GetTransactionsByUserIdAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal("Completed", updatedTransactions[0].Status);
    }
}

