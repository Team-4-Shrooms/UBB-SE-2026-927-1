using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.WebApi.DTOs;

namespace MovieApp.Web.ViewModels.TrailerScraping
{
    public class TrailerScrapingIndexViewModel
    {
        public ScrapeForm Form { get; set; } = new();
        public IEnumerable<MovieDto> AvailableMovies { get; set; } = new List<MovieDto>();
        public IEnumerable<ScrapeJobDto> RecentJobs { get; set; } = new List<ScrapeJobDto>();
    }
}
