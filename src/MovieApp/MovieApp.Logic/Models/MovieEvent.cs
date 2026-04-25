using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Logic.Models
{
    public sealed class MovieEvent
    {
        public int Id { get; set; }
        public Movie Movie { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public string PosterUrl { get; set; } = string.Empty;
        public string DisplayDate => Date.ToString("yyyy-MM-dd HH:mm");
        public string DisplayTicketPrice => TicketPrice.ToString("C");
    }
}
