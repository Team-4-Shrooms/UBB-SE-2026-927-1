using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Interface for user repository, responsible for managing user-related data such as balance and other user-specific information.
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Retrieves the current balance for a user
        /// </summary>
        /// <param name="userId">Id of the user for who we need the balance.</param>
        /// <returns></returns>
        decimal GetBalance(int userId);

        /// <summary>
        /// Retrieves the current balance for a user asynchronously.
        /// </summary>
        /// <param name="userId">Id of the user for who we need the balance.</param>
        /// <returns>A task representing the asynchronous operation, containing the user's balance.</returns>
        Task<decimal> GetBalanceAsync(int userId);

        /// <summary>
        /// Updates the user's balance to a new value. 
        /// </summary>
        /// <param name="userId">Id of the user for what we need to update the balance.</param>
        /// <param name="newBalance"></param>
        void UpdateBalance(int userId, decimal newBalance);

        /// <summary>
        /// Updates the user's balance to a new value asynchronously.
        /// </summary>
        /// <param name="userId">Id of the user for what we need to update the balance.</param>
        /// <param name="newBalance">The new balance value.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateBalanceAsync(int userId, decimal newBalance);
    }
}

