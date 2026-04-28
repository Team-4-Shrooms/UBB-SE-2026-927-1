namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Interface for user repository, responsible for managing user-related data such as balance and other user-specific information.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves the current balance for a user
        /// </summary>
        /// <param name="userId">Id of the user for who we need the balance.</param>
        /// <returns></returns>
        decimal GetBalance(int userId);

        /// <summary>
        /// Updates the user's balance to a new value. 
        /// </summary>
        /// <param name="userId">Id of the user for what we need to update the balance.</param>
        /// <param name="newBalance"></param>
        void UpdateBalance(int userId, decimal newBalance);
    }
}
