using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.DataLayer.Models
{
    public sealed class MovieEvent
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public string PosterUrl { get; set; } = string.Empty;

        public Movie Movie { get; set; }

        [NotMapped]
        public string DisplayDate => Date.ToString("yyyy-MM-dd HH:mm");
        [NotMapped]
        public string DisplayTicketPrice => TicketPrice.ToString("C");
    }
}
