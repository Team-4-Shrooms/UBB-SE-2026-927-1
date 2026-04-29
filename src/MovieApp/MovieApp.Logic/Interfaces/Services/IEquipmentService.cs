using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Services
{
    public interface IEquipmentService
    {
        Task ListItemAsync(Equipment item);
        Task PurchaseEquipmentAsync(int equipmentId, int buyerId, decimal price, string address);
    }
}

