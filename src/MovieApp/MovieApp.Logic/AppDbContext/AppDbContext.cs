using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        // Core
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }

        // Commerce
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<MovieEvent> MovieEvents { get; set; }
        public DbSet<ActiveSale> ActiveSales { get; set; }
        public DbSet<MovieReview> MovieReviews { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<OwnedMovie> OwnedMovies { get; set; }
        public DbSet<OwnedTicket> OwnedTickets { get; set; }

        // Social
        public DbSet<Reel> Reels { get; set; }
        public DbSet<MusicTrack> MusicTracks { get; set; }
        public DbSet<ScrapeJob> ScrapeJobs { get; set; }
        public DbSet<ScrapeJobLog> ScrapeJobLogs { get; set; }
        public DbSet<UserMoviePreference> UserMoviePreferences { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserReelInteraction> UserReelInteractions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1-to-1 Relationships
            modelBuilder.Entity<User>()
                .HasOne(user => user.Profile)
                .WithOne(userProfile => userProfile.User)
                .HasForeignKey<UserProfile>("UserId");

            modelBuilder.Entity<Movie>()
                .HasOne(movie => movie.ActiveSale)
                .WithOne(activeSale => activeSale.Movie)
                .HasForeignKey<ActiveSale>("MovieId");

            // Prevent Multiple Cascade Delete Paths

            // Transactions
            modelBuilder.Entity<Transaction>()
                .HasOne(transaction => transaction.Buyer)
                .WithMany(user => user.Purchases)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(transaction => transaction.Seller)
                .WithMany(user => user.Sales)
                .OnDelete(DeleteBehavior.Restrict);

            // Reviews (Restrict deleting a User from wiping out the Movie's review history)
            modelBuilder.Entity<MovieReview>()
                .HasOne(movieReview => movieReview.User)
                .WithMany().OnDelete(DeleteBehavior.Restrict);

            // Reel Interactions (Restrict User deletion from breaking Reel stats)
            modelBuilder.Entity<UserReelInteraction>()
                .HasOne(userReelInteraction => userReelInteraction.User)
                .WithMany(user => user.ReelInteractions)
                .OnDelete(DeleteBehavior.Restrict);

            // Owned Items
            modelBuilder.Entity<OwnedMovie>()
                .HasOne(ownedMovie => ownedMovie.User)
                .WithMany(user => user.OwnedMovies)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OwnedTicket>()
                .HasOne(ownedTicket => ownedTicket.User)
                .WithMany(user => user.OwnedTickets)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserMoviePreference>()
                .HasOne(userMoviePreference => userMoviePreference.User)
                .WithMany(user => user.MoviePreferences)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal Precisions (Avoid Truncation/Warnings)
            modelBuilder.Entity<User>()
                .Property(user => user.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movie>()
                .Property(movie => movie.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movie>()
                .Property(movie => movie.ActiveSaleDiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Equipment>()
                .Property(equipment => equipment.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MovieEvent>()
                .Property(movieEvent => movieEvent.TicketPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movie>()
                .Property(movie => movie.Rating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<ActiveSale>()
                .Property(activeSale => activeSale.DiscountPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Transaction>()
                .Property(transaction => transaction.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MusicTrack>()
                .Property(musicTrack => musicTrack.DurationSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reel>()
                .Property(reel => reel.FeatureDurationSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserMoviePreference>()
                .Property(preference => preference.Score)
                .HasPrecision(8, 4);

            modelBuilder.Entity<UserProfile>()
                .Property(profile => profile.AverageWatchTimeSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserProfile>()
                .Property(profile => profile.LikeToViewRatio)
                .HasPrecision(8, 4);

            modelBuilder.Entity<UserReelInteraction>()
                .Property(interaction => interaction.WatchDurationSeconds)
                .HasPrecision(18, 2);
        }
    }
}
