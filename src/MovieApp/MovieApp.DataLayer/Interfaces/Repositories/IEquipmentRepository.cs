using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing movie-related equipment.
    /// </summary>
    public interface IEquipmentRepository
    {
        /// <summary>
        /// Retrieves a list of available equipment for purchase.
        /// </summary>
        /// <returns>A list of <see cref="Equipment"/> objects representing the equipment available for purchase.</returns>
        List<Equipment> FetchAvailableEquipment();

        Task<Equipment?> GetByIdAsync(int id);
        Task AddAsync(Equipment item);
        Task AddTransactionAsync(Transaction transaction);
        Task<int> SaveChangesAsync();
    }
}

