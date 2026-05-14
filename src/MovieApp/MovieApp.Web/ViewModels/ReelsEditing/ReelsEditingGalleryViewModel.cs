namespace MovieApp.Web.ViewModels.ReelsEditing
{
    using System.Collections.Generic;

    public sealed class ReelsEditingGalleryViewModel
    {
        public IList<ReelGalleryItem> UserReels { get; set; } = new List<ReelGalleryItem>();
    }
}
