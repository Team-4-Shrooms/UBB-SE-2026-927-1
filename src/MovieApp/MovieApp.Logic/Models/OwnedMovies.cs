namespace MovieApp.Logic.Models
{
    public class OwnedMovie
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public Movie Movie { get; set; }
    }
}
