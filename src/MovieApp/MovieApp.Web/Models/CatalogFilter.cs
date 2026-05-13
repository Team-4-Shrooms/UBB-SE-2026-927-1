namespace MovieApp.Web.Models
{
    public class CatalogFilter
    {
        public string? Search { get; set; }

        public string? Genre { get; set; }

        public decimal? MinRating { get; set; }
    }
}
