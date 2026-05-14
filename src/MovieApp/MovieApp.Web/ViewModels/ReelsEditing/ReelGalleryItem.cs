namespace MovieApp.Web.ViewModels.ReelsEditing
{
    using System;

    public sealed class ReelGalleryItem
    {
        public int ReelId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public DateTime? LastEditedAt { get; set; }
    }
}
