using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    public interface IEquipmentRepository
    {
        List<Equipment> FetchAvailableEquipment();

        void ListItem(Equipment item);

        void PurchaseEquipment(int equipmentId, int buyerId, decimal price, string address);
    }
}
