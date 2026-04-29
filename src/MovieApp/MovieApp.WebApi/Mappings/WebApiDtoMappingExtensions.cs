using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs;

namespace MovieApp.WebApi.Mappings;

public static class WebApiDtoMappingExtensions
{
    public static UserReferenceDto ToReferenceDto(this User user)
    {
        return new UserReferenceDto
        {
            Id = user.Id,
            Username = user.Username ?? string.Empty,
        };
    }

    public static MovieReferenceDto ToReferenceDto(this Movie movie)
    {
        return new MovieReferenceDto
        {
            Id = movie.Id,
            Title = movie.Title ?? string.Empty,
            PosterUrl = movie.PosterUrl,
            ReleaseYear = movie.ReleaseYear,
            PrimaryGenre = movie.PrimaryGenre ?? string.Empty,
            Price = movie.Price,
        };
    }

    public static ReelReferenceDto ToReferenceDto(this Reel reel)
    {
        return new ReelReferenceDto
        {
            Id = reel.Id,
            Title = reel.Title ?? string.Empty,
            ThumbnailUrl = reel.ThumbnailUrl ?? string.Empty,
            VideoUrl = reel.VideoUrl ?? string.Empty,
        };
    }

    public static MovieEventReferenceDto ToReferenceDto(this MovieEvent movieEvent)
    {
        return new MovieEventReferenceDto
        {
            Id = movieEvent.Id,
            Title = movieEvent.Title ?? string.Empty,
            Date = movieEvent.Date,
            Location = movieEvent.Location ?? string.Empty,
            PosterUrl = movieEvent.PosterUrl ?? string.Empty,
        };
    }

    public static ScrapeJobReferenceDto ToReferenceDto(this ScrapeJob job)
    {
        return new ScrapeJobReferenceDto
        {
            Id = job.Id,
            SearchQuery = job.SearchQuery ?? string.Empty,
            Status = job.Status ?? string.Empty,
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt,
        };
    }

    public static EquipmentReferenceDto ToReferenceDto(this Equipment equipment)
    {
        return new EquipmentReferenceDto
        {
            Id = equipment.Id,
            Title = equipment.Title ?? string.Empty,
            Category = equipment.Category ?? string.Empty,
            Price = equipment.Price,
            ImageUrl = equipment.ImageUrl ?? string.Empty,
            Status = equipment.Status.ToString(),
        };
    }

    public static Equipment ToModel(this EquipmentListItemRequestBody dto)
    {
        return new Equipment
        {
            Title = dto.Title ?? string.Empty,
            Category = dto.Category ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            Condition = dto.Condition ?? string.Empty,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl ?? string.Empty,
        };
    }

    public static ActiveSaleDto ToDto(this ActiveSale sale)
    {
        return new ActiveSaleDto
        {
            Id = sale.Id,
            DiscountPercentage = sale.DiscountPercentage,
            StartTime = sale.StartTime,
            EndTime = sale.EndTime,
            Movie = sale.Movie?.ToReferenceDto(),
        };
    }

    public static MusicTrackDto ToDto(this MusicTrack track)
    {
        return new MusicTrackDto
        {
            Id = track.Id,
            TrackName = track.TrackName ?? string.Empty,
            Author = track.Author ?? string.Empty,
            AudioUrl = track.AudioUrl ?? string.Empty,
            DurationSeconds = track.DurationSeconds,
            FormattedDuration = track.FormattedDuration,
        };
    }

    public static EquipmentDto ToDto(this Equipment equipment)
    {
        return new EquipmentDto
        {
            Id = equipment.Id,
            Title = equipment.Title ?? string.Empty,
            Category = equipment.Category ?? string.Empty,
            Description = equipment.Description ?? string.Empty,
            Condition = equipment.Condition ?? string.Empty,
            Price = equipment.Price,
            ImageUrl = equipment.ImageUrl ?? string.Empty,
            Status = equipment.Status.ToString(),
            Seller = equipment.Seller?.ToReferenceDto(),
        };
    }

    public static MovieEventDto ToDto(this MovieEvent movieEvent)
    {
        return new MovieEventDto
        {
            Id = movieEvent.Id,
            Title = movieEvent.Title ?? string.Empty,
            Description = movieEvent.Description ?? string.Empty,
            Date = movieEvent.Date,
            Location = movieEvent.Location ?? string.Empty,
            TicketPrice = movieEvent.TicketPrice,
            PosterUrl = movieEvent.PosterUrl ?? string.Empty,
            Movie = movieEvent.Movie?.ToReferenceDto(),
            DisplayDate = movieEvent.DisplayDate,
            DisplayTicketPrice = movieEvent.DisplayTicketPrice,
        };
    }

    public static OwnedMovieDto ToOwnedMovieDto(this Movie movie, int? userId = null)
    {
        return new OwnedMovieDto
        {
            Movie = movie.ToReferenceDto(),
            User = userId.HasValue ? new UserReferenceDto { Id = userId.Value } : null,
        };
    }

    public static OwnedTicketDto ToOwnedTicketDto(this MovieEvent movieEvent, int? userId = null)
    {
        return new OwnedTicketDto
        {
            Event = movieEvent.ToReferenceDto(),
            User = userId.HasValue ? new UserReferenceDto { Id = userId.Value } : null,
        };
    }

    public static MovieDto ToDto(this Movie movie)
    {
        return new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title ?? string.Empty,
            Description = movie.Description ?? string.Empty,
            Rating = movie.Rating,
            Price = movie.Price,
            PrimaryGenre = movie.PrimaryGenre ?? string.Empty,
            Genre = movie.Genre ?? string.Empty,
            PosterUrl = movie.PosterUrl,
            ReleaseYear = movie.ReleaseYear,
            IsOnSale = movie.IsOnSale,
            ActiveSaleDiscountPercent = movie.ActiveSaleDiscountPercent,
            Synopsis = movie.Synopsis ?? string.Empty,
            HasActiveSale = movie.HasActiveSale,
            OriginalPriceText = movie.OriginalPriceText,
            DiscountedPriceText = movie.DiscountedPriceText,
        };
    }

    public static UserReelInteractionDto ToDto(this UserReelInteraction interaction, int? userId = null, int? reelId = null)
    {
        return new UserReelInteractionDto
        {
            Id = interaction.Id,
            IsLiked = interaction.IsLiked,
            WatchDurationSeconds = interaction.WatchDurationSeconds,
            WatchPercentage = interaction.WatchPercentage,
            ViewedAt = interaction.ViewedAt,
            User = interaction.User?.ToReferenceDto() ?? (userId.HasValue ? new UserReferenceDto { Id = userId.Value } : null),
            Reel = interaction.Reel?.ToReferenceDto() ?? (reelId.HasValue ? new ReelReferenceDto { Id = reelId.Value } : null),
        };
    }

    public static UserMoviePreferenceDto ToDto(this UserMoviePreference preference)
    {
        return new UserMoviePreferenceDto
        {
            Id = preference.Id,
            Score = preference.Score,
            LastModified = preference.LastModified,
            ChangeFromPreviousValue = preference.ChangeFromPreviousValue,
            User = preference.User?.ToReferenceDto(),
            Movie = preference.Movie?.ToReferenceDto(),
        };
    }

    public static UserProfileDto ToDto(this UserProfile profile, int? userId = null)
    {
        return new UserProfileDto
        {
            Id = profile.Id,
            TotalLikes = profile.TotalLikes,
            TotalWatchTimeSeconds = profile.TotalWatchTimeSeconds,
            AverageWatchTimeSeconds = profile.AverageWatchTimeSeconds,
            TotalClipsViewed = profile.TotalClipsViewed,
            LikeToViewRatio = profile.LikeToViewRatio,
            LastUpdated = profile.LastUpdated,
            User = profile.User?.ToReferenceDto() ?? (userId.HasValue ? new UserReferenceDto { Id = userId.Value } : null),
        };
    }

    public static MovieReviewDto ToDto(this MovieReview review, int? movieId = null)
    {
        return new MovieReviewDto
        {
            Id = review.Id,
            StarRating = review.StarRating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            Movie = review.Movie?.ToReferenceDto() ?? (movieId.HasValue ? new MovieReferenceDto { Id = movieId.Value } : null),
            User = review.User?.ToReferenceDto(),
            DisplayStarRating = review.DisplayStarRating,
            DisplayCreatedAt = review.DisplayCreatedAt,
        };
    }

    public static ReelDto ToDto(this Reel reel)
    {
        return new ReelDto
        {
            Id = reel.Id,
            VideoUrl = reel.VideoUrl ?? string.Empty,
            ThumbnailUrl = reel.ThumbnailUrl ?? string.Empty,
            Title = reel.Title ?? string.Empty,
            Caption = reel.Caption ?? string.Empty,
            FeatureDurationSeconds = reel.FeatureDurationSeconds,
            CropDataJson = reel.CropDataJson,
            BackgroundMusicId = reel.BackgroundMusicId,
            Source = reel.Source ?? string.Empty,
            Genre = reel.Genre,
            CreatedAt = reel.CreatedAt,
            LastEditedAt = reel.LastEditedAt,
            Movie = reel.Movie?.ToReferenceDto(),
            CreatorUser = reel.CreatorUser?.ToReferenceDto(),
        };
    }

    public static TransactionDto ToDto(this Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type ?? string.Empty,
            Status = transaction.Status ?? string.Empty,
            Timestamp = transaction.Timestamp,
            ShippingAddress = transaction.ShippingAddress,
            Buyer = transaction.Buyer?.ToReferenceDto(),
            Seller = transaction.Seller?.ToReferenceDto(),
            Equipment = transaction.Equipment?.ToReferenceDto(),
            Movie = transaction.Movie?.ToReferenceDto(),
            Event = transaction.Event?.ToReferenceDto(),
            DisplayTimestamp = transaction.DisplayTimestamp,
        };
    }

    public static Transaction ToModel(this LogTransactionRequestBody dto)
    {
        return new Transaction
        {
            Amount = dto.Amount,
            Type = dto.Type ?? string.Empty,
            Status = dto.Status ?? string.Empty,
            Timestamp = dto.Timestamp,
            ShippingAddress = dto.ShippingAddress,
            Equipment = dto.EquipmentId.HasValue ? new Equipment { Id = dto.EquipmentId.Value } : null,
            Movie = dto.MovieId.HasValue ? new Movie { Id = dto.MovieId.Value } : null,
            Event = dto.EventId.HasValue ? new MovieEvent { Id = dto.EventId.Value } : null,
        };
    }

    public static ScrapeJobDto ToDto(this ScrapeJob job)
    {
        return new ScrapeJobDto
        {
            Id = job.Id,
            SearchQuery = job.SearchQuery ?? string.Empty,
            MaxResults = job.MaxResults,
            Status = job.Status ?? string.Empty,
            MoviesFound = job.MoviesFound,
            ReelsCreated = job.ReelsCreated,
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt,
            ErrorMessage = job.ErrorMessage,
            Logs = job.Logs?.Select(log => log.ToDto()).ToList() ?? [],
        };
    }

    public static ScrapeJob ToModel(this ScrapeJobRequestBody dto)
    {
        return new ScrapeJob
        {
            SearchQuery = dto.SearchQuery ?? string.Empty,
            MaxResults = dto.MaxResults,
            Status = dto.Status ?? string.Empty,
            MoviesFound = dto.MoviesFound,
            ReelsCreated = dto.ReelsCreated,
            StartedAt = dto.StartedAt,
            CompletedAt = dto.CompletedAt,
            ErrorMessage = dto.ErrorMessage,
        };
    }

    public static ScrapeJobLog ToModel(this AddLogEntryRequestBody dto)
    {
        return new ScrapeJobLog
        {
            Level = dto.Level ?? string.Empty,
            Message = dto.Message ?? string.Empty,
            Timestamp = dto.Timestamp,
            ScrapeJob = new ScrapeJob { Id = dto.ScrapeJobId },
        };
    }

    public static ScrapeJobLogDto ToDto(this ScrapeJobLog log)
    {
        return new ScrapeJobLogDto
        {
            Id = log.Id,
            Level = log.Level ?? string.Empty,
            Message = log.Message ?? string.Empty,
            Timestamp = log.Timestamp,
            ScrapeJob = log.ScrapeJob?.ToReferenceDto(),
        };
    }

    public static DashboardStatsDto ToDto(this DashboardStatsModel stats)
    {
        return new DashboardStatsDto
        {
            TotalMovies = stats.TotalMovies,
            TotalReels = stats.TotalReels,
            TotalJobs = stats.TotalJobs,
            RunningJobs = stats.RunningJobs,
            CompletedJobs = stats.CompletedJobs,
            FailedJobs = stats.FailedJobs,
        };
    }

    public static MoviePreferenceDisplayDto ToDto(this MoviePreferenceDisplay preference)
    {
        return new MoviePreferenceDisplayDto
        {
            MovieId = preference.MovieId,
            Title = preference.Title ?? string.Empty,
            Score = preference.Score,
            IsBestMovie = preference.IsBestMovie,
        };
    }

    public static Reel ToModel(this InsertReelRequestBody dto)
    {
        return new Reel
        {
            VideoUrl = dto.VideoUrl ?? string.Empty,
            ThumbnailUrl = dto.ThumbnailUrl ?? string.Empty,
            Title = dto.Title ?? string.Empty,
            Caption = dto.Caption ?? string.Empty,
            FeatureDurationSeconds = dto.FeatureDurationSeconds,
            CropDataJson = dto.CropDataJson,
            BackgroundMusicId = dto.BackgroundMusicId,
            Source = dto.Source ?? string.Empty,
            Genre = dto.Genre,
            CreatedAt = dto.CreatedAt,
            LastEditedAt = dto.LastEditedAt,
            Movie = new Movie { Id = dto.MovieId },
            CreatorUser = new User { Id = dto.CreatorUserId },
        };
    }

    public static UserProfile ToModel(this UpsertProfileRequestBody dto)
    {
        return new UserProfile
        {
            TotalLikes = dto.TotalLikes,
            TotalWatchTimeSeconds = dto.TotalWatchTimeSeconds,
            AverageWatchTimeSeconds = dto.AverageWatchTimeSeconds,
            TotalClipsViewed = dto.TotalClipsViewed,
            LikeToViewRatio = dto.LikeToViewRatio,
            LastUpdated = dto.LastUpdated,
        };
    }

    public static UserReelInteraction ToModel(this InsertInteractionRequestBody dto)
    {
        return new UserReelInteraction
        {
            IsLiked = dto.IsLiked,
            WatchDurationSeconds = dto.WatchDurationSeconds,
            WatchPercentage = dto.WatchPercentage,
            ViewedAt = dto.ViewedAt,
            Reel = new Reel { Id = dto.ReelId },
        };
    }

    public static OwnedMovieDto ToDto(this OwnedMovie ownedMovie)
    {
        return new OwnedMovieDto
        {
            Id = ownedMovie.Id,
            PurchaseDate = ownedMovie.PurchaseDate,
            User = ownedMovie.User?.ToReferenceDto(),
            Movie = ownedMovie.Movie?.ToReferenceDto(),
        };
    }

    public static OwnedTicketDto ToDto(this OwnedTicket ownedTicket)
    {
        return new OwnedTicketDto
        {
            Id = ownedTicket.Id,
            PurchaseDate = ownedTicket.PurchaseDate,
            User = ownedTicket.User?.ToReferenceDto(),
            Event = ownedTicket.Event?.ToReferenceDto(),
        };
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Balance = user.Balance,
        };
    }
}
