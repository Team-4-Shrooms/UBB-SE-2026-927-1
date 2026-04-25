using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MovieApp.Logic.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Shrooms Domain
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<MovieEvent> MovieEvents { get; set; }
        public DbSet<ActiveSale> ActiveSales { get; set; }
        public DbSet<MovieReview> MovieReviews { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<OwnedMovie> OwnedMovies { get; set; }
        public DbSet<OwnedTicket> OwnedTickets { get; set; }

        // PureCaffeine Domain
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

            // Configure 1-to-1 relationship for User and UserProfile explicitly
            // (EF Core sometimes needs help knowing which side is the principal in 1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>("UserId"); // Shadow property
        }
    }
}