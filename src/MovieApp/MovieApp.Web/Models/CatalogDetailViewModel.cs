using MovieApp.DataLayer.Models;

namespace MovieApp.Web.Models
{
    public class CatalogDetailViewModel
    {
        public Movie Movie { get; set; } = null!;

        public IEnumerable<MovieReview> Reviews { get; set; } = new List<MovieReview>();

        public int[] StarRatingBuckets { get; set; } = new int[5];

        public AddReviewForm Form { get; set; } = new AddReviewForm();
    }
}
