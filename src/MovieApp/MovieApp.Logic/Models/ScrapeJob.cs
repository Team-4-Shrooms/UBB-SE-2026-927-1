using System;

namespace MovieApp.Logic.Models
{
    public class ScrapeJob
    {
        public int Id { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
        public int MaxResults { get; set; }
        public string Status { get; set; } = "pending";
        public int MoviesFound { get; set; }
        public int ReelsCreated { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
