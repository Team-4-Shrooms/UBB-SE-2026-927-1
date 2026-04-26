using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.Tests.Data
{
    public class DataSeederTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsUsers()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsUsers));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            int userCount = await context.Users.CountAsync();
            Assert.Equal(6, userCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsCorrectUsernames()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsCorrectUsernames));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            List<string> usernames = await context.Users
                .Select(u => u.Username)
                .ToListAsync();

            Assert.Contains("User1", usernames);
            Assert.Contains("Alice", usernames);
            Assert.Contains("Bob", usernames);
            Assert.Contains("Carol", usernames);
            Assert.Contains("Dave", usernames);
            Assert.Contains("Eve", usernames);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsMovies()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsMovies));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            int movieCount = await context.Movies.CountAsync();
            Assert.Equal(38, movieCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsFirstEightMoviesCorrectly()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsFirstEightMoviesCorrectly));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            List<string> movieTitles = await context.Movies
                .Select(m => m.Title)
                .ToListAsync();

            Assert.Contains("Inception", movieTitles);
            Assert.Contains("The Dark Knight", movieTitles);
            Assert.Contains("Interstellar", movieTitles);
            Assert.Contains("The Matrix", movieTitles);
            Assert.Contains("Parasite", movieTitles);
            Assert.Contains("La La Land", movieTitles);
            Assert.Contains("Whiplash", movieTitles);
            Assert.Contains("The Grand Budapest Hotel", movieTitles);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsMusicTracks()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsMusicTracks));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            int trackCount = await context.MusicTracks.CountAsync();
            Assert.Equal(5, trackCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsMusicTracksWithCorrectNames()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsMusicTracksWithCorrectNames));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            List<string> trackNames = await context.MusicTracks
                .Select(t => t.TrackName)
                .ToListAsync();

            Assert.Contains("Epic Cinematic Theme", trackNames);
            Assert.Contains("Upbeat Pop Track", trackNames);
            Assert.Contains("Dramatic Orchestral", trackNames);
            Assert.Contains("Chill Lo-Fi Beats", trackNames);
            Assert.Contains("Action Packed Rock", trackNames);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsReels()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsReels));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            int reelCount = await context.Reels.CountAsync();
            Assert.Equal(10, reelCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_reelsBelongToUser1()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_reelsBelongToUser1));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            User? user1 = await context.Users.FirstOrDefaultAsync(u => u.Username == "User1");
            Assert.NotNull(user1);

            int user1ReelCount = await context.Reels
                .CountAsync(r => r.CreatorUser.Id == user1.Id);

            Assert.Equal(10, user1ReelCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsUserMoviePreferences()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsUserMoviePreferences));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            int preferenceCount = await context.UserMoviePreferences.CountAsync();
            Assert.True(preferenceCount > 0);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_user1HasEightPreferences()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_user1HasEightPreferences));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            User? user1 = await context.Users.FirstOrDefaultAsync(u => u.Username == "User1");
            Assert.NotNull(user1);

            int user1PreferenceCount = await context.UserMoviePreferences
                .CountAsync(p => p.User.Id == user1.Id);

            Assert.Equal(8, user1PreferenceCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_allPreferencesHavePositiveChangeFromPrevious()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_allPreferencesHavePositiveChangeFromPrevious));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool anyNonPositive = await context.UserMoviePreferences
                .AnyAsync(p => p.ChangeFromPreviousValue <= 0);

            Assert.False(anyNonPositive);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsUserProfiles()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsUserProfiles));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            int profileCount = await context.UserProfiles.CountAsync();
            Assert.Equal(6, profileCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_eachUserHasOneProfile()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_eachUserHasOneProfile));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            List<User> users = await context.Users.ToListAsync();

            foreach (User user in users)
            {
                int profileCount = await context.UserProfiles
                    .CountAsync(p => p.User.Id == user.Id);

                Assert.Equal(1, profileCount);
            }
        }

        [Fact]
        public async Task SeedAsync_calledTwice_doesNotDuplicateData()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_calledTwice_doesNotDuplicateData));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();
            await seeder.SeedAsync();

            int userCount = await context.Users.CountAsync();
            int movieCount = await context.Movies.CountAsync();
            int trackCount = await context.MusicTracks.CountAsync();

            Assert.Equal(6, userCount);
            Assert.Equal(38, movieCount);
            Assert.Equal(5, trackCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_tournamentPoolForUser1HasEightMovies()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_tournamentPoolForUser1HasEightMovies));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            User? user1 = await context.Users.FirstOrDefaultAsync(u => u.Username == "User1");
            Assert.NotNull(user1);

            int tournamentPoolSize = await context.UserMoviePreferences
                .CountAsync(p => p.User.Id == user1.Id && p.ChangeFromPreviousValue > 0);

            Assert.Equal(8, tournamentPoolSize);
        }
    }
}
