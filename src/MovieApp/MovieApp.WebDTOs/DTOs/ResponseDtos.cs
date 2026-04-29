namespace MovieApp.WebDTOs.DTOs;

public sealed class ActiveSaleDto
{
    public int Id { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public MovieReferenceDto? Movie { get; set; }
}

public sealed class MusicTrackDto
{
    public int Id { get; set; }
    public string TrackName { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string AudioUrl { get; set; } = string.Empty;
    public decimal DurationSeconds { get; set; }
    public string FormattedDuration { get; set; } = string.Empty;
}

public sealed class EquipmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public UserReferenceDto? Seller { get; set; }
}

public sealed class MovieEventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal TicketPrice { get; set; }
    public string PosterUrl { get; set; } = string.Empty;
    public MovieReferenceDto? Movie { get; set; }
    public string DisplayDate { get; set; } = string.Empty;
    public string DisplayTicketPrice { get; set; } = string.Empty;
}

public sealed class OwnedMovieDto
{
    public int Id { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public UserReferenceDto? User { get; set; }
    public MovieReferenceDto? Movie { get; set; }
}

public sealed class OwnedTicketDto
{
    public int Id { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public UserReferenceDto? User { get; set; }
    public MovieEventReferenceDto? Event { get; set; }
}

public sealed class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public decimal Price { get; set; }
    public string PrimaryGenre { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public int ReleaseYear { get; set; }
    public bool IsOnSale { get; set; }
    public decimal? ActiveSaleDiscountPercent { get; set; }
    public string Synopsis { get; set; } = string.Empty;
    public bool HasActiveSale { get; set; }
    public string OriginalPriceText { get; set; } = string.Empty;
    public string DiscountedPriceText { get; set; } = string.Empty;
}

public sealed class UserReelInteractionDto
{
    public long Id { get; set; }
    public bool IsLiked { get; set; }
    public decimal WatchDurationSeconds { get; set; }
    public decimal WatchPercentage { get; set; }
    public DateTime ViewedAt { get; set; }
    public UserReferenceDto? User { get; set; }
    public ReelReferenceDto? Reel { get; set; }
}

public sealed class UserMoviePreferenceDto
{
    public int Id { get; set; }
    public decimal Score { get; set; }
    public DateTime LastModified { get; set; }
    public int? ChangeFromPreviousValue { get; set; }
    public UserReferenceDto? User { get; set; }
    public MovieReferenceDto? Movie { get; set; }
}

public sealed class UserProfileDto
{
    public int Id { get; set; }
    public int TotalLikes { get; set; }
    public long TotalWatchTimeSeconds { get; set; }
    public decimal AverageWatchTimeSeconds { get; set; }
    public int TotalClipsViewed { get; set; }
    public decimal LikeToViewRatio { get; set; }
    public DateTime LastUpdated { get; set; }
    public UserReferenceDto? User { get; set; }
}

public sealed class MovieReviewDto
{
    public int Id { get; set; }
    public decimal StarRating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public MovieReferenceDto? Movie { get; set; }
    public UserReferenceDto? User { get; set; }
    public string DisplayStarRating { get; set; } = string.Empty;
    public string DisplayCreatedAt { get; set; } = string.Empty;
}

public sealed class ReelDto
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
    public MovieReferenceDto? Movie { get; set; }
    public UserReferenceDto? CreatorUser { get; set; }
}

public sealed class TransactionDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? ShippingAddress { get; set; }
    public UserReferenceDto? Buyer { get; set; }
    public UserReferenceDto? Seller { get; set; }
    public EquipmentReferenceDto? Equipment { get; set; }
    public MovieReferenceDto? Movie { get; set; }
    public MovieEventReferenceDto? Event { get; set; }
    public string DisplayTimestamp { get; set; } = string.Empty;
}

public sealed class ScrapeJobDto
{
    public int Id { get; set; }
    public string SearchQuery { get; set; } = string.Empty;
    public int MaxResults { get; set; }
    public string Status { get; set; } = string.Empty;
    public int MoviesFound { get; set; }
    public int ReelsCreated { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public List<ScrapeJobLogDto> Logs { get; set; } = [];
}

public sealed class ScrapeJobLogDto
{
    public int Id { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public ScrapeJobReferenceDto? ScrapeJob { get; set; }
}

public sealed class DashboardStatsDto
{
    public int TotalMovies { get; set; }
    public int TotalReels { get; set; }
    public int TotalJobs { get; set; }
    public int RunningJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int FailedJobs { get; set; }
}

public sealed class MoviePreferenceDisplayDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public bool IsBestMovie { get; set; }
}

public sealed class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
