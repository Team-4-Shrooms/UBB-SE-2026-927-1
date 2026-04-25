using System;

namespace MovieApp.Logic.Models
{
    public class ScrapeJobLogModel
    {
        public int Id { get; set; } // ?
        public ScrapeJobModel ScrapeJob { get; set; }
        public string Level { get; set; } = "Info";
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
