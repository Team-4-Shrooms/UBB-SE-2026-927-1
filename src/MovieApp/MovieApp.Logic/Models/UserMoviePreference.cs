namespace MovieApp.Logic.Models
{
    public class UserMoviePreference
    {
        public int Id { get; set; }
        public decimal Score { get; set; }
        public DateTime LastModified { get; set; }
        public int? ChangeFromPreviousValue { get; set; }

        public User User { get; set; }
        public Movie Movie { get; set; }
    }
}
