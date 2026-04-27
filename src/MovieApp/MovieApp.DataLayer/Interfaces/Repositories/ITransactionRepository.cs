using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    /// <summary>
    /// The repository interface for managing transactions related to movie purchases, rentals, and event ticket sales.
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Logs a new transaction in the system.
        /// </summary>
        /// <param name="transaction">The transaction we need to log</param>
        void LogTransaction(Transaction transaction);

        /// <summary>
        /// Retreives a list of transactions associated with a specific user, allowing for tracking of purchase history and sales activity.
        /// </summary>
        /// <param name="userId">The user for what we need the transactions</param>
        /// <returns>A list of <see cref="Transaction"/> objects representing the transactions associated with the specified user.</returns>
        List<Transaction> GetTransactionsByUserId(int userId);

        /// <summary>
        /// Updates the status of the specified transaction.
        /// </summary>
        /// <param name="transactionId">The unique identifier of the transaction to update. Must be a valid, existing transaction ID.</param>
        /// <param name="newStatus">The new status to assign to the transaction.</param>
        void UpdateTransactionStatus(int transactionId, string newStatus);
    }
}
