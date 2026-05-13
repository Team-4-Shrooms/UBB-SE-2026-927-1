using MovieApp.DataLayer.Models;

namespace MovieApp.Web.Models
{
    public class CatalogIndexViewModel
    {
        public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();

        public IEnumerable<string> Genres { get; set; } = new List<string>();

        public CatalogFilter Filter { get; set; } = new CatalogFilter();
    }
}
