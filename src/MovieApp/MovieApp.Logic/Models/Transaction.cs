using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Logic.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public User Buyer { get; set; }

        public User? Seller { get; set; } // ?

        public Equipment? Equipment { get; set; }
        public Movie? Movie { get; set; }
        public MovieEvent? Event { get; set; } // ?

        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string? ShippingAddress { get; set; }

        // formatting
        public string DisplayTimestamp => Timestamp.ToString("g");
        public string DisplayType => MovieShop.Services.TransactionTypeMapper.ToDisplayString(Type);
        public string DisplayStatus => MovieShop.Services.TransactionTypeMapper.StatusToDisplayString(Status);
        public string DisplayAmount => MovieShop.Services.TransactionTypeMapper.FormatAmount(Amount);
    }
}