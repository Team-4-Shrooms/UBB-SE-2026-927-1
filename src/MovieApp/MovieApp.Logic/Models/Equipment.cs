using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Logic.Models
{
    public enum EquipmentStatus
    {
        Available,
        Sold,
        Pending
    }

    public class Equipment
    {
        public int Id { get; set; }
        public User Seller { get; set; } // ?
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;
    }
}
