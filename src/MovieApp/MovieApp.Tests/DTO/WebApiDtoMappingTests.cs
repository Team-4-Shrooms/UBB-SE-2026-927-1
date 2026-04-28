using System.Text.Json;
using MovieApp.DataLayer.DTO.WebAPI;
using MovieApp.DataLayer.Models;

namespace MovieApp.Tests.DTO;

public sealed class WebApiDtoMappingTests
{
    [Fact]
    public void CatalogAndMarketplaceDtos_AvoidRawEntityGraphs()
    {
        Movie movie = BuildMovie();
        ActiveSale activeSale = new ActiveSale
        {
            Id = 11,
            DiscountPercentage = 15m,
            StartTime = new DateTime(2026, 1, 1),
            EndTime = new DateTime(2026, 1, 2),
            Movie = movie,
        };
        movie.ActiveSale = activeSale;

        MovieEvent movieEvent = new MovieEvent
        {
            Id = 12,
            Title = "Premiere",
            Description = "Opening night",
            Date = new DateTime(2026, 2, 3, 20, 0, 0),
            Location = "Cinema 1",
            TicketPrice = 21.50m,
            PosterUrl = "poster-event",
            Movie = movie,
        };

        User seller = BuildUser(2, "seller");
        Equipment equipment = new Equipment
        {
            Id = 13,
            Title = "Camera",
            Category = "Video",
            Description = "4K camera",
            Condition = "Used",
            Price = 900m,
            ImageUrl = "image",
            Status = EquipmentStatus.Available,
            Seller = seller,
        };

        MovieDto movieDto = movie.ToDto();
        ActiveSaleDto activeSaleDto = activeSale.ToDto();
        MovieEventDto movieEventDto = movieEvent.ToDto();
        EquipmentDto equipmentDto = equipment.ToDto();
        OwnedMovieDto ownedMovieDto = movie.ToOwnedMovieDto(1);
        OwnedTicketDto ownedTicketDto = movieEvent.ToOwnedTicketDto(1);

        Assert.Equal(movie.Id, movieDto.Id);
        Assert.Equal("Action", movieDto.PrimaryGenre);
        Assert.True(movieDto.HasActiveSale);
        Assert.Equal(movie.Id, activeSaleDto.Movie?.Id);
        Assert.Equal(movie.Id, movieEventDto.Movie?.Id);
        Assert.Equal(seller.Id, equipmentDto.Seller?.Id);
        Assert.Equal(movie.Id, ownedMovieDto.Movie?.Id);
        Assert.Equal(movieEvent.Id, ownedTicketDto.Event?.Id);

        AssertDtoSerializesWithoutLeaks(movieDto, "ActiveSale", "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(activeSaleDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(movieEventDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(equipmentDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(ownedMovieDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(ownedTicketDto, "PasswordHash", "Email");
    }

    [Fact]
    public void SocialAndInteractionDtos_AvoidSensitiveFields()
    {
        User user = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        Reel reel = new Reel
        {
            Id = 20,
            VideoUrl = "video",
            ThumbnailUrl = "thumb",
            Title = "Highlight",
            Caption = "Caption",
            FeatureDurationSeconds = 18m,
            CropDataJson = "{}",
            BackgroundMusicId = 7,
            Source = "Upload",
            Genre = "Action",
            CreatedAt = new DateTime(2026, 3, 4),
            LastEditedAt = new DateTime(2026, 3, 5),
            Movie = movie,
            CreatorUser = user,
        };

        UserReelInteraction interaction = new UserReelInteraction
        {
            Id = 21,
            IsLiked = true,
            WatchDurationSeconds = 13m,
            WatchPercentage = 72m,
            ViewedAt = new DateTime(2026, 3, 6),
            User = user,
            Reel = reel,
        };

        UserMoviePreference preference = new UserMoviePreference
        {
            Id = 22,
            Score = 91m,
            LastModified = new DateTime(2026, 3, 7),
            ChangeFromPreviousValue = 6,
            User = user,
            Movie = movie,
        };

        UserProfile profile = new UserProfile
        {
            Id = 23,
            TotalLikes = 4,
            TotalWatchTimeSeconds = 80,
            AverageWatchTimeSeconds = 20m,
            TotalClipsViewed = 8,
            LikeToViewRatio = 0.5m,
            LastUpdated = new DateTime(2026, 3, 8),
            User = user,
        };

        MovieReview review = new MovieReview
        {
            Id = 24,
            StarRating = 8.5m,
            Comment = "Good",
            CreatedAt = new DateTime(2026, 3, 9),
            Movie = movie,
            User = user,
        };

        Transaction transaction = new Transaction
        {
            Id = 25,
            Amount = -14.99m,
            Type = "MoviePurchase",
            Status = "Completed",
            Timestamp = new DateTime(2026, 3, 10),
            ShippingAddress = "123 Street",
            Buyer = user,
            Seller = BuildUser(4, "seller"),
            Movie = movie,
            Event = new MovieEvent { Id = 26, Title = "Event", Description = "", Date = new DateTime(2026, 3, 11), Location = "Hall", TicketPrice = 10m, PosterUrl = "poster" },
            Equipment = new Equipment { Id = 27, Title = "Tripod", Category = "Video", Description = "", Condition = "New", Price = 35m, ImageUrl = "tripod", Status = EquipmentStatus.Available },
        };

        ReelDto reelDto = reel.ToDto();
        UserReelInteractionDto interactionDto = interaction.ToDto();
        UserMoviePreferenceDto preferenceDto = preference.ToDto();
        UserProfileDto profileDto = profile.ToDto(3);
        MovieReviewDto reviewDto = review.ToDto(movie.Id);
        TransactionDto transactionDto = transaction.ToDto();

        Assert.Equal(reel.Id, reelDto.Id);
        Assert.Equal(user.Id, reelDto.CreatorUser?.Id);
        Assert.Equal(user.Id, interactionDto.User?.Id);
        Assert.Equal(reel.Id, interactionDto.Reel?.Id);
        Assert.Equal(movie.Id, preferenceDto.Movie?.Id);
        Assert.Equal(user.Id, profileDto.User?.Id);
        Assert.Equal(movie.Id, reviewDto.Movie?.Id);
        Assert.Equal(user.Id, transactionDto.Buyer?.Id);

        AssertDtoSerializesWithoutLeaks(reelDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(interactionDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(preferenceDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(profileDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(reviewDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(transactionDto, "PasswordHash", "Email");
    }

    [Fact]
    public void AdminDtos_SerializeWithoutReferenceLoops()
    {
        ScrapeJob job = new ScrapeJob
        {
            Id = 31,
            SearchQuery = "sci-fi",
            MaxResults = 10,
            Status = "running",
            MoviesFound = 2,
            ReelsCreated = 1,
            StartedAt = new DateTime(2026, 4, 1),
        };

        ScrapeJobLog log = new ScrapeJobLog
        {
            Id = 32,
            Level = "Info",
            Message = "Started",
            Timestamp = new DateTime(2026, 4, 2),
            ScrapeJob = job,
        };
        job.Logs.Add(log);

        MusicTrack track = new MusicTrack
        {
            Id = 33,
            TrackName = "Theme",
            Author = "Composer",
            AudioUrl = "audio",
            DurationSeconds = 125m,
        };

        DashboardStatsModel stats = new DashboardStatsModel
        {
            TotalMovies = 5,
            TotalReels = 6,
            TotalJobs = 7,
            RunningJobs = 1,
            CompletedJobs = 4,
            FailedJobs = 2,
        };

        MoviePreferenceDisplay preferenceDisplay = new MoviePreferenceDisplay
        {
            MovieId = 34,
            Title = "Top Pick",
            Score = 98m,
            IsBestMovie = true,
        };

        ScrapeJobDto jobDto = job.ToDto();
        ScrapeJobLogDto logDto = log.ToDto();
        MusicTrackDto trackDto = track.ToDto();
        DashboardStatsDto statsDto = stats.ToDto();
        MoviePreferenceDisplayDto preferenceDisplayDto = preferenceDisplay.ToDto();

        Assert.Single(jobDto.Logs);
        Assert.Equal(job.Id, jobDto.Logs[0].ScrapeJob?.Id);
        Assert.Equal(job.Id, logDto.ScrapeJob?.Id);
        Assert.Equal("0:05", trackDto.FormattedDuration);
        Assert.Equal(stats.TotalJobs, statsDto.TotalJobs);
        Assert.Equal(preferenceDisplay.MovieId, preferenceDisplayDto.MovieId);

        AssertDtoSerializesWithoutLeaks(jobDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(logDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(trackDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(statsDto, "PasswordHash", "Email");
        AssertDtoSerializesWithoutLeaks(preferenceDisplayDto, "PasswordHash", "Email");
    }

    private static Movie BuildMovie()
    {
        return new Movie
        {
            Id = 1,
            Title = "Movie",
            Description = "Description",
            Rating = 8.1m,
            Price = 14.99m,
            PrimaryGenre = "Action",
            PosterUrl = "poster",
            ReleaseYear = 2026,
            IsOnSale = true,
            ActiveSaleDiscountPercent = 20m,
            Synopsis = "Synopsis",
        };
    }

    private static User BuildUser(int id, string username)
    {
        return new User
        {
            Id = id,
            Username = username,
            Email = $"{username}@example.com",
            PasswordHash = "hash",
            Balance = 100m,
        };
    }

    private static void AssertDtoSerializesWithoutLeaks<T>(T value, params string[] forbiddenTerms)
    {
        string json = JsonSerializer.Serialize(value);

        foreach (string forbiddenTerm in forbiddenTerms)
        {
            Assert.DoesNotContain(forbiddenTerm, json, StringComparison.OrdinalIgnoreCase);
        }
    }
}