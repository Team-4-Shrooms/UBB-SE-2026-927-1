using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        void LogTransaction(Transaction transaction);

        List<Transaction> GetTransactionsByUserId(int userId);

        void UpdateTransactionStatus(int transactionId, string newStatus);
    }
}
