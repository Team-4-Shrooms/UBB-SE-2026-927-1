using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
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

        /// <summary>
        /// Lists a piece of equipment for sale by adding it to the marketplace.
        /// </summary>
        /// <param name="item">The <see cref="Equipment"/> object representing the item to be listed for sale.</param>
        void ListItem(Equipment item);

        /// <summary>
        /// Purchases a piece of equipment from the marketplace, transferring ownership to the buyer and removing it from the list of available items.
        /// </summary>
        /// <param name="equipmentId">The unique identifier of the equipment being purchased. Must correspond to an available item in the marketplace.</param>
        /// <param name="buyerId">The unique identifier of the user making the purchase. Must correspond to an existing user.</param>
        /// <param name="price">The price at which the equipment is being purchased. Must be a non-negative value and should match the listed price of the equipment.</param>
        /// <param name="address">The address to which the equipment should be shipped. Must be a valid shipping address.</param>
        void PurchaseEquipment(int equipmentId, int buyerId, decimal price, string address);
    }
}
