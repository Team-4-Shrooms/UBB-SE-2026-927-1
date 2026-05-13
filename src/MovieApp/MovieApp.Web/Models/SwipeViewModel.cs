using MovieApp.DataLayer.Models;

namespace MovieApp.Web.Models
{
    public class SwipeCardViewModel
    {
        public Movie Movie { get; set; } = null!;
        public int LikedCount { get; set; }
        public int DislikedCount { get; set; }
    }

    public class SwipeSummaryViewModel
    {
        public int LikedCount { get; set; }
        public int DislikedCount { get; set; }
    }
}
