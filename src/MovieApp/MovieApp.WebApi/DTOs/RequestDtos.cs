namespace MovieApp.WebApi.DTOs;

public sealed class EquipmentListItemRequestBody
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int SellerId { get; set; }
}

public sealed class LogTransactionRequestBody
{
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? ShippingAddress { get; set; }
    public int BuyerId { get; set; }
    public int? SellerId { get; set; }
    public int? EquipmentId { get; set; }
    public int? MovieId { get; set; }
    public int? EventId { get; set; }
}

public sealed class InsertReelRequestBody
{
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
    public int MovieId { get; set; }
    public int CreatorUserId { get; set; }
}

public sealed class ScrapeJobRequestBody
{
    public string SearchQuery { get; set; } = string.Empty;
    public int MaxResults { get; set; }
    public string Status { get; set; } = string.Empty;
    public int MoviesFound { get; set; }
    public int ReelsCreated { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class AddLogEntryRequestBody
{
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int ScrapeJobId { get; set; }
}

public sealed class UpsertProfileRequestBody
{
    public int TotalLikes { get; set; }
    public long TotalWatchTimeSeconds { get; set; }
    public decimal AverageWatchTimeSeconds { get; set; }
    public int TotalClipsViewed { get; set; }
    public decimal LikeToViewRatio { get; set; }
    public DateTime LastUpdated { get; set; }
    public int UserId { get; set; }
}

public sealed class InsertInteractionRequestBody
{
    public bool IsLiked { get; set; }
    public decimal WatchDurationSeconds { get; set; }
    public decimal WatchPercentage { get; set; }
    public DateTime ViewedAt { get; set; }
    public int UserId { get; set; }
    public int ReelId { get; set; }
}

public sealed class PurchaseEquipmentRequestBody
{
    public int BuyerId { get; set; }
    public decimal Price { get; set; }
    public string Address { get; set; } = string.Empty;
}

public sealed class PurchaseTicketRequestBody
{
    public int UserId { get; set; }
}

public sealed class UpdateViewDataRequestBody
{
    public decimal WatchDurationSeconds { get; set; }
    public decimal WatchPercentage { get; set; }
}

public sealed class PurchaseMovieRequestBody
{
    public int UserId { get; set; }
    public decimal FinalPrice { get; set; }
}

public sealed class BoostMovieScoreRequestBody
{
    public decimal ScoreBoost { get; set; }
}

public sealed class InsertPreferenceRequestBody
{
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public decimal Score { get; set; }
}

public sealed class UpdatePreferenceRequestBody
{
    public decimal Boost { get; set; }
}

public sealed class UpdateReelEditsRequestBody
{
    public string CropDataJson { get; set; } = string.Empty;
    public int? BackgroundMusicId { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
}

public sealed class AddReviewRequestBody
{
    public int MovieId { get; set; }
    public int UserId { get; set; }
    public int StarRating { get; set; }
    public string? Comment { get; set; }
}

public sealed class GetReviewCountsRequestBody
{
    public IEnumerable<int> MovieIds { get; set; } = [];
}

public sealed class UpdateTransactionStatusRequestBody
{
    public string NewStatus { get; set; } = string.Empty;
}

public sealed class UpdateBalanceRequestBody
{
    public decimal NewBalance { get; set; }
}
