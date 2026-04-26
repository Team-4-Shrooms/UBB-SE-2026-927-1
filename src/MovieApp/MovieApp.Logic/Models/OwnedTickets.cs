namespace MovieApp.Logic.Models
{
    public class OwnedTicket
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public MovieEvent Event { get; set; }
    }
}
