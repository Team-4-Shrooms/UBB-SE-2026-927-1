namespace MovieApp.DataLayer.Models
{
    public class ActiveSale
    {
        public int Id { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Movie Movie { get; set; }

        public bool IsExpired()
        { 
            return DateTime.Now > EndTime; 
        }
    }
}
