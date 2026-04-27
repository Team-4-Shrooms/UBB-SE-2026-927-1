using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Data
{
    public interface IMovieAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Movie> Movies { get; }
        DbSet<Equipment> Equipment { get; }
        DbSet<MovieEvent> MovieEvents { get; }
        DbSet<ActiveSale> ActiveSales { get; }
        DbSet<MovieReview> MovieReviews { get; }
        DbSet<Transaction> Transactions { get; }
        DbSet<OwnedMovie> OwnedMovies { get; }
        DbSet<OwnedTicket> OwnedTickets { get; }
        DbSet<Reel> Reels { get; }
        DbSet<MusicTrack> MusicTracks { get; }
        DbSet<ScrapeJob> ScrapeJobs { get; }
        DbSet<ScrapeJobLog> ScrapeJobLogs { get; }
        DbSet<UserMoviePreference> UserMoviePreferences { get; }
        DbSet<UserProfile> UserProfiles { get; }
        DbSet<UserReelInteraction> UserReelInteractions { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}