using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Web.ViewModels.TrailerScraping
{
    public class ScrapeForm
    {
        public int MovieId { get; set; }
        public string? YouTubeUrl { get; set; }
        public int MaxResults { get; set; } = 5;
    }
}
