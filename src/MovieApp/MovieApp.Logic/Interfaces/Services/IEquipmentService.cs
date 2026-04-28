using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IEquipmentService
    {
        Task ListItemAsync(Equipment item);
        Task PurchaseEquipmentAsync(int equipmentId, int buyerId, decimal price, string address);
    }
}
