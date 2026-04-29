using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Interfaces.Services;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepo;
        private readonly IUserRepository _userRepo;

        public EquipmentService(IEquipmentRepository equipmentRepo, IUserRepository userRepo)
        {
            _equipmentRepo = equipmentRepo;
            _userRepo = userRepo;
        }
        public async Task ListItemAsync(Equipment item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            item.Status = EquipmentStatus.Available;

            await _equipmentRepo.AddAsync(item);
            await _equipmentRepo.SaveChangesAsync();
        }

        public async Task PurchaseEquipmentAsync(int equipmentId, int buyerId, decimal price, string address)
        {
            if (buyerId <= 0) throw new InvalidOperationException("You must be logged in to purchase.");

            var equipment = await _equipmentRepo.GetByIdAsync(equipmentId)
                            ?? throw new KeyNotFoundException("Equipment not found.");

            var buyer = await _userRepo.GetUserByIdAsync(buyerId)
                        ?? throw new KeyNotFoundException("Buyer not found.");

            if (equipment.Status != EquipmentStatus.Available)
                throw new InvalidOperationException("This item is no longer available.");

            if (buyer.Balance < price)
                throw new InvalidOperationException("Insufficient balance to complete purchase.");

            buyer.Balance -= price;
            equipment.Status = EquipmentStatus.Sold;

            var transaction = new Transaction
            {
                Buyer = buyer,
                Seller = equipment.Seller,
                Equipment = equipment,
                Amount = -price,
                Type = "EquipmentPurchase",
                Status = "Completed",
                Timestamp = DateTime.UtcNow,
                ShippingAddress = address
            };

            await _equipmentRepo.AddTransactionAsync(transaction);

            await _equipmentRepo.SaveChangesAsync();
        }
    }
}

