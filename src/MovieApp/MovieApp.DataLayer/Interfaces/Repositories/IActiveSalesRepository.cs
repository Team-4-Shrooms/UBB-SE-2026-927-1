using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing active sales and discounts for movies.
    /// </summary>
    public interface IActiveSalesRepository
    {
        /// <summary>
        /// Retrieves a list of currently active sales.
        /// </summary>
        /// <returns>A list of <see cref="ActiveSale"/> objects representing the currently active sales. The list will be empty if there are no active sales.</returns>
        Task<List<ActiveSale>> GetCurrentSalesAsync();
    }
}

