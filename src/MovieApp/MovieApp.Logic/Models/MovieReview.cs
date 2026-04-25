using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Logic.Models
{
    public sealed class MovieReview
    {
        public int Id { get; set; }
        public Movie Movie { get; set; }
        public User User { get; set; }
        public string Username { get; set; } = string.Empty;
        public decimal StarRating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string DisplayStarRating => $"{StarRating:0.0}/10";
        public string DisplayCreatedAt => CreatedAt.ToString("yyyy-MM-dd HH:mm");
    }
}
