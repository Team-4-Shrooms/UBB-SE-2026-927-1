using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MovieApp.DataLayer.Models
{
    public partial class Reel : ObservableObject
    {
        public int Id { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public decimal FeatureDurationSeconds { get; set; }
        public string? CropDataJson { get; set; }
        public int? BackgroundMusicId { get; set; }
        public string Source { get; set; } = string.Empty;
        public string? Genre { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastEditedAt { get; set; }

        public Movie Movie { get; set; } = null!;
        public User CreatorUser { get; set; } = null!;

        public ICollection<UserMoviePreference> MoviePreferences { get; set; } = new List<UserMoviePreference>();

        // ── Client-side state (changes at runtime, needs UI notification) ──

        // The [property: NotMapped] attribute tells Entity Framework to ignore this,
        // preventing database crashes, while still generating the UI property!

        [property: NotMapped]
        [ObservableProperty]
        private bool isLiked;

        [property: NotMapped]
        [ObservableProperty]
        private int likeCount;
    }
}
