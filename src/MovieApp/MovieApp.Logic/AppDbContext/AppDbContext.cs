using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MovieApp.Logic.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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

        // Social / PureCaffeine
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
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>("UserId");

            modelBuilder.Entity<Movie>()
                .HasOne(m => m.ActiveSale)
                .WithOne(a => a.Movie)
                .HasForeignKey<ActiveSale>("MovieId");

            // Prevent Multiple Cascade Delete Paths

            // Transactions
            modelBuilder.Entity<Transaction>().HasOne(t => t.Buyer).WithMany(u => u.Purchases).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Transaction>().HasOne(t => t.Seller).WithMany(u => u.Sales).OnDelete(DeleteBehavior.Restrict);

            // Reviews (Restrict deleting a User from wiping out the Movie's review history)
            modelBuilder.Entity<MovieReview>().HasOne(r => r.User).WithMany().OnDelete(DeleteBehavior.Restrict);

            // Reel Interactions (Restrict User deletion from breaking Reel stats)
            modelBuilder.Entity<UserReelInteraction>().HasOne(i => i.User).WithMany(u => u.ReelInteractions).OnDelete(DeleteBehavior.Restrict);

            // Owned Items
            modelBuilder.Entity<OwnedMovie>().HasOne(o => o.User).WithMany(u => u.OwnedMovies).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OwnedTicket>().HasOne(o => o.User).WithMany(u => u.OwnedTickets).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserMoviePreference>().HasOne(p => p.User).WithMany(u => u.MoviePreferences).OnDelete(DeleteBehavior.Restrict);

            // Decimal Precisions (Avoid Truncation/Warnings)
            modelBuilder.Entity<User>().Property(u => u.Balance).HasPrecision(18, 2);
            modelBuilder.Entity<Movie>().Property(m => m.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Movie>().Property(m => m.ActiveSaleDiscountPercent).HasPrecision(5, 2);
            modelBuilder.Entity<Equipment>().Property(e => e.Price).HasPrecision(18, 2);
            modelBuilder.Entity<MovieEvent>().Property(e => e.TicketPrice).HasPrecision(18, 2);
            modelBuilder.Entity<ActiveSale>().Property(a => a.DiscountPercentage).HasPrecision(5, 2);
            modelBuilder.Entity<Transaction>().Property(t => t.Amount).HasPrecision(18, 2);
        }
    }
}