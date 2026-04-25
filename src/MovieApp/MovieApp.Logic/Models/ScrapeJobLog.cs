using System;

namespace MovieApp.Logic.Models
{
    public class ScrapeJobLog
    {
        public int Id { get; set; } // ?
        public ScrapeJob ScrapeJob { get; set; }
        public string Level { get; set; } = "Info";
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
