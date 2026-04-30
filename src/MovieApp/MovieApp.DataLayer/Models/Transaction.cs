using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.DataLayer.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string? ShippingAddress { get; set; }

        public User Buyer { get; set; }
        public User? Seller { get; set; }
        public Equipment? Equipment { get; set; }
        public Movie? Movie { get; set; }
        public MovieEvent? Event { get; set; }

        [NotMapped]
        public string DisplayTimestamp => Timestamp.ToString("g");

        [NotMapped]
        public string DisplayType => Type ?? string.Empty;

        [NotMapped]
        public string DisplayStatus => Status ?? string.Empty;

        [NotMapped]
        public string DisplayAmount => Amount.ToString("C");
    }
}