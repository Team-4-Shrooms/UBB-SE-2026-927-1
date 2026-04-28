using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class EquipmentRepository : IEquipmentRepository
    {
        private const string EquipmentPurchaseTransactionType = "EquipmentPurchase";
        private const string CompletedTransactionStatus = "Completed";

        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;

        public EquipmentRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        public List<Equipment> FetchAvailableEquipment()
        {
            return _context.Equipment
                .AsNoTracking()
                .Include(equipment => equipment.Seller)
                .Where(equipment => equipment.Status == EquipmentStatus.Available)
                .ToList();
        }

        public void ListItem(Equipment item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Status = EquipmentStatus.Available;
            _context.Equipment.Add(item);
            _context.SaveChanges();
        }

        public void PurchaseEquipment(int equipmentId, int buyerId, decimal price, string address)
        {
            if (buyerId <= 0)
            {
                throw new InvalidOperationException("You must be logged in to purchase.");
            }

            Equipment? equipment = _context.Equipment
                .Include(candidate => candidate.Seller)
                .FirstOrDefault(candidate => candidate.Id == equipmentId);
            if (equipment is null)
            {
                throw new InvalidOperationException("Equipment not found.");
            }
            User? buyer = _context.Users.FirstOrDefault(candidate => candidate.Id == buyerId);
            if (buyer is null)
            {
                throw new InvalidOperationException("Buyer not found.");
            }

            buyer.Balance -= price;
            equipment.Status = EquipmentStatus.Sold;

            Transaction transaction = new Transaction
            {
                Buyer = buyer,
                Seller = equipment.Seller,
                Equipment = equipment,
                Amount = -price,
                Type = EquipmentPurchaseTransactionType,
                Status = CompletedTransactionStatus,
                Timestamp = DateTime.UtcNow,
                ShippingAddress = address
            };
            _context.Transactions.Add(transaction);

            _context.SaveChanges();
        }
    }
}
