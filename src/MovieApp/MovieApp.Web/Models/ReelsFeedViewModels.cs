namespace MovieApp.Web.Models
{
    public class ReelsFeedViewModel
    {
        public List<ReelDisplayItem> Reels { get; set; } = new();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public bool IsLoading { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class ReelDisplayItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Caption { get; set; }

        public string VideoUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public int LikeCount { get; set; }

        public bool IsLikedByMe { get; set; } 
    }
}
