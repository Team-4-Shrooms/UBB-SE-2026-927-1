using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class TransactionRepository : ITransactionRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;
        private DbSet<Transaction> Transactions => _context.Transactions;

        public TransactionRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        public void LogTransaction(Transaction transaction)
        {
            if (transaction is null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            Transactions.Add(transaction);
            _context.SaveChanges();
        }

        public List<Transaction> GetTransactionsByUserId(int userId)
        {
            return _context.Transactions
                .Where(transaction =>
                    transaction.Buyer.Id == userId ||
                    (transaction.Seller != null && transaction.Seller.Id == userId))
                .OrderByDescending(transaction => transaction.Timestamp)
                .ToList();
        }

        public void UpdateTransactionStatus(int transactionId, string newStatus)
        {
            Transaction? transaction = _context.Transactions
                .FirstOrDefault(candidate => candidate.Id == transactionId);
            if (transaction is null)
            {
                return;
            }

            transaction.Status = newStatus;
            _context.SaveChanges();
        }
    }
}
