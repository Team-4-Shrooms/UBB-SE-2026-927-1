using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<Transaction>> GetTransactionsByUserIdAsync(int userId, int? page = null, int? pageSize = null)
        {
            var transactions = _transactionRepository.GetTransactionsByUserId(userId);
            if (page.HasValue && pageSize.HasValue && page.Value > 0 && pageSize.Value > 0)
            {
                transactions = transactions.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
            }
            return await Task.FromResult(transactions);
        }

        public async Task LogTransactionAsync(Transaction transaction)
        {
            _transactionRepository.LogTransaction(transaction);
            await Task.CompletedTask;
        }

        public async Task UpdateTransactionStatusAsync(int transactionId, string newStatus)
        {
            _transactionRepository.UpdateTransactionStatus(transactionId, newStatus);
            await Task.CompletedTask;
        }
    }
}
