using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsByUserIdAsync(int userId, int? page = null, int? pageSize = null);
        Task LogTransactionAsync(Transaction transaction);
        Task UpdateTransactionStatusAsync(int transactionId, string newStatus);
    }
}
