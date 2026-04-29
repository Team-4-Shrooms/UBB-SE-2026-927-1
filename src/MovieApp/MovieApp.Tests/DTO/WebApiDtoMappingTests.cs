using System.Text.Json;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Models;

namespace MovieApp.Tests.DTO;

public sealed class WebApiDtoMappingTests
{
    // ── MovieDto ──

    [Fact]
    public void MovieToDto_ValidMovie_MapsCorrectId()
    {
        Movie movie = BuildMovie();

        MovieDto movieDto = movie.ToDto();

        Assert.Equal(movie.Id, movieDto.Id);
    }

    [Fact]
    public void MovieToDto_ValidMovie_MapsCorrectPrimaryGenre()
    {
        Movie movie = BuildMovie();

        MovieDto movieDto = movie.ToDto();

        Assert.Equal("Action", movieDto.PrimaryGenre);
    }

    [Fact]
    public void MovieToDto_MovieWithActiveSale_HasActiveSaleIsTrue()
    {
        Movie movie = BuildMovie();
        movie.ActiveSale = new ActiveSale
        {
            Id = 11,
            DiscountPercentage = 15m,
            StartTime = new DateTime(2026, 1, 1),
            EndTime = new DateTime(2026, 1, 2),
            Movie = movie,
        };

        MovieDto movieDto = movie.ToDto();

        Assert.True(movieDto.HasActiveSale);
    }

    [Fact]
    public void MovieToDto_ValidMovie_SerializedJsonDoesNotContainPasswordHash()
    {
        Movie movie = BuildMovie();

        MovieDto movieDto = movie.ToDto();
        string serializedJson = JsonSerializer.Serialize(movieDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MovieToDto_ValidMovie_SerializedJsonDoesNotContainEmail()
    {
        Movie movie = BuildMovie();

        MovieDto movieDto = movie.ToDto();
        string serializedJson = JsonSerializer.Serialize(movieDto);

        Assert.DoesNotContain("email", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── ActiveSaleDto ──

    [Fact]
    public void ActiveSaleToDto_ValidSale_MapsCorrectMovieId()
    {
        Movie movie = BuildMovie();
        ActiveSale activeSale = BuildActiveSale(movie);

        ActiveSaleDto activeSaleDto = activeSale.ToDto();

        Assert.Equal(movie.Id, activeSaleDto.Movie?.Id);
    }

    [Fact]
    public void ActiveSaleToDto_ValidSale_SerializedJsonDoesNotContainPasswordHash()
    {
        Movie movie = BuildMovie();
        ActiveSale activeSale = BuildActiveSale(movie);

        ActiveSaleDto activeSaleDto = activeSale.ToDto();
        string serializedJson = JsonSerializer.Serialize(activeSaleDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── MovieEventDto ──

    [Fact]
    public void MovieEventToDto_ValidEvent_MapsCorrectMovieId()
    {
        Movie movie = BuildMovie();
        MovieEvent movieEvent = BuildMovieEvent(movie);

        MovieEventDto movieEventDto = movieEvent.ToDto();

        Assert.Equal(movie.Id, movieEventDto.Movie?.Id);
    }

    [Fact]
    public void MovieEventToDto_ValidEvent_SerializedJsonDoesNotContainPasswordHash()
    {
        Movie movie = BuildMovie();
        MovieEvent movieEvent = BuildMovieEvent(movie);

        MovieEventDto movieEventDto = movieEvent.ToDto();
        string serializedJson = JsonSerializer.Serialize(movieEventDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── EquipmentDto ──

    [Fact]
    public void EquipmentToDto_ValidEquipment_MapsCorrectSellerId()
    {
        User seller = BuildUser(2, "seller");
        Equipment equipment = BuildEquipment(seller);

        EquipmentDto equipmentDto = equipment.ToDto();

        Assert.Equal(seller.Id, equipmentDto.Seller?.Id);
    }

    [Fact]
    public void EquipmentToDto_ValidEquipment_SerializedJsonDoesNotContainPasswordHash()
    {
        User seller = BuildUser(2, "seller");
        Equipment equipment = BuildEquipment(seller);

        EquipmentDto equipmentDto = equipment.ToDto();
        string serializedJson = JsonSerializer.Serialize(equipmentDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EquipmentToDto_ValidEquipment_SerializedJsonDoesNotContainEmail()
    {
        User seller = BuildUser(2, "seller");
        Equipment equipment = BuildEquipment(seller);

        EquipmentDto equipmentDto = equipment.ToDto();
        string serializedJson = JsonSerializer.Serialize(equipmentDto);

        Assert.DoesNotContain("email", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── OwnedMovieDto ──

    [Fact]
    public void MovieToOwnedMovieDto_ValidMovie_MapsCorrectMovieId()
    {
        Movie movie = BuildMovie();

        OwnedMovieDto ownedMovieDto = movie.ToOwnedMovieDto(1);

        Assert.Equal(movie.Id, ownedMovieDto.Movie?.Id);
    }

    [Fact]
    public void MovieToOwnedMovieDto_ValidMovie_SerializedJsonDoesNotContainPasswordHash()
    {
        Movie movie = BuildMovie();

        OwnedMovieDto ownedMovieDto = movie.ToOwnedMovieDto(1);
        string serializedJson = JsonSerializer.Serialize(ownedMovieDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── OwnedTicketDto ──

    [Fact]
    public void MovieEventToOwnedTicketDto_ValidEvent_MapsCorrectEventId()
    {
        Movie movie = BuildMovie();
        MovieEvent movieEvent = BuildMovieEvent(movie);

        OwnedTicketDto ownedTicketDto = movieEvent.ToOwnedTicketDto(1);

        Assert.Equal(movieEvent.Id, ownedTicketDto.Event?.Id);
    }

    [Fact]
    public void MovieEventToOwnedTicketDto_ValidEvent_SerializedJsonDoesNotContainPasswordHash()
    {
        Movie movie = BuildMovie();
        MovieEvent movieEvent = BuildMovieEvent(movie);

        OwnedTicketDto ownedTicketDto = movieEvent.ToOwnedTicketDto(1);
        string serializedJson = JsonSerializer.Serialize(ownedTicketDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── ReelDto ──

    [Fact]
    public void ReelToDto_ValidReel_MapsCorrectId()
    {
        User creator = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        Reel reel = BuildReel(movie, creator);

        ReelDto reelDto = reel.ToDto();

        Assert.Equal(reel.Id, reelDto.Id);
    }

    [Fact]
    public void ReelToDto_ValidReel_MapsCorrectCreatorUserId()
    {
        User creator = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        Reel reel = BuildReel(movie, creator);

        ReelDto reelDto = reel.ToDto();

        Assert.Equal(creator.Id, reelDto.CreatorUser?.Id);
    }

    [Fact]
    public void ReelToDto_ValidReel_SerializedJsonDoesNotContainPasswordHash()
    {
        User creator = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        Reel reel = BuildReel(movie, creator);

        ReelDto reelDto = reel.ToDto();
        string serializedJson = JsonSerializer.Serialize(reelDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ReelToDto_ValidReel_SerializedJsonDoesNotContainEmail()
    {
        User creator = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        Reel reel = BuildReel(movie, creator);

        ReelDto reelDto = reel.ToDto();
        string serializedJson = JsonSerializer.Serialize(reelDto);

        Assert.DoesNotContain("email", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── UserReelInteractionDto ──

    [Fact]
    public void InteractionToDto_ValidInteraction_MapsCorrectUserId()
    {
        User user = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        Reel reel = BuildReel(movie, user);
        UserReelInteraction interaction = BuildInteraction(user, reel);

        UserReelInteractionDto interactionDto = interaction.ToDto();

        Assert.Equal(user.Id, interactionDto.User?.Id);
    }

    [Fact]
    public void InteractionToDto_ValidInteraction_MapsCorrectReelId()
    {
        User user = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        Reel reel = BuildReel(movie, user);
        UserReelInteraction interaction = BuildInteraction(user, reel);

        UserReelInteractionDto interactionDto = interaction.ToDto();

        Assert.Equal(reel.Id, interactionDto.Reel?.Id);
    }

    [Fact]
    public void InteractionToDto_ValidInteraction_SerializedJsonDoesNotContainPasswordHash()
    {
        User user = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        Reel reel = BuildReel(movie, user);
        UserReelInteraction interaction = BuildInteraction(user, reel);

        UserReelInteractionDto interactionDto = interaction.ToDto();
        string serializedJson = JsonSerializer.Serialize(interactionDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── UserMoviePreferenceDto ──

    [Fact]
    public void PreferenceToDto_ValidPreference_MapsCorrectMovieId()
    {
        User user = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        UserMoviePreference preference = BuildPreference(user, movie);

        UserMoviePreferenceDto preferenceDto = preference.ToDto();

        Assert.Equal(movie.Id, preferenceDto.Movie?.Id);
    }

    [Fact]
    public void PreferenceToDto_ValidPreference_SerializedJsonDoesNotContainPasswordHash()
    {
        User user = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        UserMoviePreference preference = BuildPreference(user, movie);

        UserMoviePreferenceDto preferenceDto = preference.ToDto();
        string serializedJson = JsonSerializer.Serialize(preferenceDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── UserProfileDto ──

    [Fact]
    public void ProfileToDto_ValidProfile_MapsCorrectUserId()
    {
        User user = BuildUser(3, "viewer");
        UserProfile profile = BuildProfile(user);

        UserProfileDto profileDto = profile.ToDto(user.Id);

        Assert.Equal(user.Id, profileDto.User?.Id);
    }

    [Fact]
    public void ProfileToDto_ValidProfile_SerializedJsonDoesNotContainPasswordHash()
    {
        User user = BuildUser(3, "viewer");
        UserProfile profile = BuildProfile(user);

        UserProfileDto profileDto = profile.ToDto(user.Id);
        string serializedJson = JsonSerializer.Serialize(profileDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── MovieReviewDto ──

    [Fact]
    public void ReviewToDto_ValidReview_MapsCorrectMovieId()
    {
        User user = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        MovieReview review = BuildReview(movie, user);

        MovieReviewDto reviewDto = review.ToDto(movie.Id);

        Assert.Equal(movie.Id, reviewDto.Movie?.Id);
    }

    [Fact]
    public void ReviewToDto_ValidReview_SerializedJsonDoesNotContainPasswordHash()
    {
        User user = BuildUser(3, "viewer");
        Movie movie = BuildMovie();
        MovieReview review = BuildReview(movie, user);

        MovieReviewDto reviewDto = review.ToDto(movie.Id);
        string serializedJson = JsonSerializer.Serialize(reviewDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── TransactionDto ──

    [Fact]
    public void TransactionToDto_ValidTransaction_MapsCorrectBuyerId()
    {
        User buyer = BuildUser(3, "viewer");
        Transaction transaction = BuildTransaction(buyer);

        TransactionDto transactionDto = transaction.ToDto();

        Assert.Equal(buyer.Id, transactionDto.Buyer?.Id);
    }

    [Fact]
    public void TransactionToDto_ValidTransaction_SerializedJsonDoesNotContainPasswordHash()
    {
        User buyer = BuildUser(3, "viewer");
        Transaction transaction = BuildTransaction(buyer);

        TransactionDto transactionDto = transaction.ToDto();
        string serializedJson = JsonSerializer.Serialize(transactionDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TransactionToDto_ValidTransaction_SerializedJsonDoesNotContainEmail()
    {
        User buyer = BuildUser(3, "viewer");
        Transaction transaction = BuildTransaction(buyer);

        TransactionDto transactionDto = transaction.ToDto();
        string serializedJson = JsonSerializer.Serialize(transactionDto);

        Assert.DoesNotContain("email", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── ScrapeJobDto ──

    [Fact]
    public void ScrapeJobToDto_JobWithOneLog_LogsCountIsOne()
    {
        ScrapeJob scrapeJob = BuildScrapeJob();
        ScrapeJobLog scrapeLog = BuildScrapeJobLog(scrapeJob);
        scrapeJob.Logs.Add(scrapeLog);

        ScrapeJobDto scrapeJobDto = scrapeJob.ToDto();

        Assert.Single(scrapeJobDto.Logs);
    }

    [Fact]
    public void ScrapeJobToDto_JobWithOneLog_FirstLogMapsCorrectScrapeJobId()
    {
        ScrapeJob scrapeJob = BuildScrapeJob();
        ScrapeJobLog scrapeLog = BuildScrapeJobLog(scrapeJob);
        scrapeJob.Logs.Add(scrapeLog);

        ScrapeJobDto scrapeJobDto = scrapeJob.ToDto();

        Assert.Equal(scrapeJob.Id, scrapeJobDto.Logs[0].ScrapeJob?.Id);
    }

    [Fact]
    public void ScrapeJobToDto_ValidJob_SerializedJsonDoesNotContainPasswordHash()
    {
        ScrapeJob scrapeJob = BuildScrapeJob();

        ScrapeJobDto scrapeJobDto = scrapeJob.ToDto();
        string serializedJson = JsonSerializer.Serialize(scrapeJobDto);

        Assert.DoesNotContain("PasswordHash", serializedJson, StringComparison.OrdinalIgnoreCase);
    }

    // ── ScrapeJobLogDto ──

    [Fact]
    public void ScrapeJobLogToDto_ValidLog_MapsCorrectScrapeJobId()
    {
        ScrapeJob scrapeJob = BuildScrapeJob();
        ScrapeJobLog scrapeLog = BuildScrapeJobLog(scrapeJob);

        ScrapeJobLogDto scrapeLogDto = scrapeLog.ToDto();

        Assert.Equal(scrapeJob.Id, scrapeLogDto.ScrapeJob?.Id);
    }

    // ── MusicTrackDto ──

    [Fact]
    public void MusicTrackToDto_DurationIs125Seconds_FormattedDurationIsTwoMinutesFiveSeconds()
    {
        MusicTrack musicTrack = new MusicTrack
        {
            Id = 33,
            TrackName = "Theme",
            Author = "Composer",
            AudioUrl = "audio",
            DurationSeconds = 125m,
        };

        MusicTrackDto musicTrackDto = musicTrack.ToDto();

        Assert.Equal("2:05", musicTrackDto.FormattedDuration);
    }

    // ── DashboardStatsDto ──

    [Fact]
    public void DashboardStatsToDto_ValidStats_MapsCorrectTotalJobs()
    {
        DashboardStatsModel statsModel = new DashboardStatsModel
        {
            TotalMovies = 5,
            TotalReels = 6,
            TotalJobs = 7,
            RunningJobs = 1,
            CompletedJobs = 4,
            FailedJobs = 2,
        };

        DashboardStatsDto statsDto = statsModel.ToDto();

        Assert.Equal(7, statsDto.TotalJobs);
    }

    // ── MoviePreferenceDisplayDto ──

    [Fact]
    public void MoviePreferenceDisplayToDto_ValidPreference_MapsCorrectMovieId()
    {
        MoviePreferenceDisplay preferenceDisplay = new MoviePreferenceDisplay
        {
            MovieId = 34,
            Title = "Top Pick",
            Score = 98m,
            IsBestMovie = true,
        };

        MoviePreferenceDisplayDto preferenceDisplayDto = preferenceDisplay.ToDto();

        Assert.Equal(34, preferenceDisplayDto.MovieId);
    }

    // ── Builders ──

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

    private static User BuildUser(int userId, string username)
    {
        return new User
        {
            Id = userId,
            Username = username,
            Email = $"{username}@example.com",
            PasswordHash = "hash",
            Balance = 100m,
        };
    }

    private static ActiveSale BuildActiveSale(Movie movie)
    {
        return new ActiveSale
        {
            Id = 11,
            DiscountPercentage = 15m,
            StartTime = new DateTime(2026, 1, 1),
            EndTime = new DateTime(2026, 1, 2),
            Movie = movie,
        };
    }

    private static MovieEvent BuildMovieEvent(Movie movie)
    {
        return new MovieEvent
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
    }

    private static Equipment BuildEquipment(User seller)
    {
        return new Equipment
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
    }

    private static Reel BuildReel(Movie movie, User creator)
    {
        return new Reel
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
            CreatorUser = creator,
        };
    }

    private static UserReelInteraction BuildInteraction(User user, Reel reel)
    {
        return new UserReelInteraction
        {
            Id = 21,
            IsLiked = true,
            WatchDurationSeconds = 13m,
            WatchPercentage = 72m,
            ViewedAt = new DateTime(2026, 3, 6),
            User = user,
            Reel = reel,
        };
    }

    private static UserMoviePreference BuildPreference(User user, Movie movie)
    {
        return new UserMoviePreference
        {
            Id = 22,
            Score = 91m,
            LastModified = new DateTime(2026, 3, 7),
            ChangeFromPreviousValue = 6,
            User = user,
            Movie = movie,
        };
    }

    private static UserProfile BuildProfile(User user)
    {
        return new UserProfile
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
    }

    private static MovieReview BuildReview(Movie movie, User user)
    {
        return new MovieReview
        {
            Id = 24,
            StarRating = 8.5m,
            Comment = "Good",
            CreatedAt = new DateTime(2026, 3, 9),
            Movie = movie,
            User = user,
        };
    }

    private static Transaction BuildTransaction(User buyer)
    {
        return new Transaction
        {
            Id = 25,
            Amount = -14.99m,
            Type = "MoviePurchase",
            Status = "Completed",
            Timestamp = new DateTime(2026, 3, 10),
            ShippingAddress = "123 Street",
            Buyer = buyer,
            Seller = BuildUser(4, "txseller"),
            Movie = BuildMovie(),
        };
    }

    private static ScrapeJob BuildScrapeJob()
    {
        return new ScrapeJob
        {
            Id = 31,
            SearchQuery = "sci-fi",
            MaxResults = 10,
            Status = "running",
            MoviesFound = 2,
            ReelsCreated = 1,
            StartedAt = new DateTime(2026, 4, 1),
        };
    }

    private static ScrapeJobLog BuildScrapeJobLog(ScrapeJob scrapeJob)
    {
        return new ScrapeJobLog
        {
            Id = 32,
            Level = "Info",
            Message = "Started",
            Timestamp = new DateTime(2026, 4, 2),
            ScrapeJob = scrapeJob,
        };
    }
}
