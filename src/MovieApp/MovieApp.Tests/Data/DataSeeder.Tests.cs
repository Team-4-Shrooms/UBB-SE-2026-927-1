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
        public async Task SeedAsync_emptyDatabase_seedsSixUsers()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsSixUsers));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            int userCount = await context.Users.CountAsync();

            Assert.Equal(6, userCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsUser1()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsUser1));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Users.AnyAsync(user => user.Username == "User1");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsAlice()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsAlice));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Users.AnyAsync(user => user.Username == "Alice");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsBob()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsBob));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Users.AnyAsync(user => user.Username == "Bob");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsCarol()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsCarol));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Users.AnyAsync(user => user.Username == "Carol");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsDave()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsDave));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Users.AnyAsync(user => user.Username == "Dave");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsEve()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsEve));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Users.AnyAsync(user => user.Username == "Eve");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsThirtyEightMovies()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsThirtyEightMovies));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            int movieCount = await context.Movies.CountAsync();

            Assert.Equal(38, movieCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsInception()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsInception));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Movies.AnyAsync(movie => movie.Title == "Inception");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsTheDarkKnight()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsTheDarkKnight));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Movies.AnyAsync(movie => movie.Title == "The Dark Knight");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsInterstellar()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsInterstellar));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Movies.AnyAsync(movie => movie.Title == "Interstellar");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsTheMatrix()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsTheMatrix));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Movies.AnyAsync(movie => movie.Title == "The Matrix");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsParasite()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsParasite));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Movies.AnyAsync(movie => movie.Title == "Parasite");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsLaLaLand()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsLaLaLand));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Movies.AnyAsync(movie => movie.Title == "La La Land");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsWhiplash()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsWhiplash));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Movies.AnyAsync(movie => movie.Title == "Whiplash");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsTheGrandBudapestHotel()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsTheGrandBudapestHotel));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.Movies.AnyAsync(movie => movie.Title == "The Grand Budapest Hotel");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsFiveMusicTracks()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsFiveMusicTracks));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            int trackCount = await context.MusicTracks.CountAsync();

            Assert.Equal(5, trackCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsEpicCinematicTheme()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsEpicCinematicTheme));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.MusicTracks.AnyAsync(track => track.TrackName == "Epic Cinematic Theme");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsUpbeatPopTrack()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsUpbeatPopTrack));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.MusicTracks.AnyAsync(track => track.TrackName == "Upbeat Pop Track");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsDramaticOrchestral()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsDramaticOrchestral));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.MusicTracks.AnyAsync(track => track.TrackName == "Dramatic Orchestral");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsChillLoFiBeats()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsChillLoFiBeats));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.MusicTracks.AnyAsync(track => track.TrackName == "Chill Lo-Fi Beats");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_containsActionPackedRock()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_containsActionPackedRock));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool exists = await context.MusicTracks.AnyAsync(track => track.TrackName == "Action Packed Rock");

            Assert.True(exists);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsTenReels()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsTenReels));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            int reelCount = await context.Reels.CountAsync();

            Assert.Equal(10, reelCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_allReelsBelongToUser1()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_allReelsBelongToUser1));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            User? user1 = await context.Users.FirstOrDefaultAsync(user => user.Username == "User1");

            int user1ReelCount = await context.Reels
                .CountAsync(reel => reel.CreatorUser.Id == user1!.Id);

            Assert.Equal(10, user1ReelCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_user1IsNotNull()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_user1IsNotNull));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            User? user1 = await context.Users.FirstOrDefaultAsync(user => user.Username == "User1");

            Assert.NotNull(user1);
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

            User? user1 = await context.Users.FirstOrDefaultAsync(user => user.Username == "User1");

            int preferenceCount = await context.UserMoviePreferences
                .CountAsync(preference => preference.User.Id == user1!.Id);

            Assert.Equal(8, preferenceCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_allPreferencesHavePositiveChangeFromPrevious()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_allPreferencesHavePositiveChangeFromPrevious));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool anyNonPositive = await context.UserMoviePreferences
                .AnyAsync(preference => preference.ChangeFromPreviousValue <= 0);

            Assert.False(anyNonPositive);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_seedsSixUserProfiles()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_seedsSixUserProfiles));

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
                    .CountAsync(profile => profile.User.Id == user.Id);

                Assert.Equal(1, profileCount);
            }
        }

        [Fact]
        public async Task SeedAsync_calledTwice_doesNotDuplicateUsers()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_calledTwice_doesNotDuplicateUsers));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();
            await seeder.SeedAsync();

            int userCount = await context.Users.CountAsync();

            Assert.Equal(6, userCount);
        }

        [Fact]
        public async Task SeedAsync_calledTwice_doesNotDuplicateMovies()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_calledTwice_doesNotDuplicateMovies));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();
            await seeder.SeedAsync();

            int movieCount = await context.Movies.CountAsync();

            Assert.Equal(38, movieCount);
        }

        [Fact]
        public async Task SeedAsync_calledTwice_doesNotDuplicateMusicTracks()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_calledTwice_doesNotDuplicateMusicTracks));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();
            await seeder.SeedAsync();

            int trackCount = await context.MusicTracks.CountAsync();

            Assert.Equal(5, trackCount);
        }

        [Fact]
        public async Task SeedAsync_calledTwice_doesNotDuplicateReels()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_calledTwice_doesNotDuplicateReels));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();
            await seeder.SeedAsync();

            int reelCount = await context.Reels.CountAsync();

            Assert.Equal(10, reelCount);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_tournamentPoolForUser1HasEightMovies()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_tournamentPoolForUser1HasEightMovies));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            User? user1 = await context.Users.FirstOrDefaultAsync(user => user.Username == "User1");

            int tournamentPoolSize = await context.UserMoviePreferences
                .CountAsync(preference => preference.User.Id == user1!.Id && preference.ChangeFromPreviousValue > 0);

            Assert.Equal(8, tournamentPoolSize);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_user1BalanceIsHundred()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_user1BalanceIsHundred));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            User? user1 = await context.Users.FirstOrDefaultAsync(user => user.Username == "User1");

            Assert.Equal(100m, user1!.Balance);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_allMoviesHaveNonZeroPrice()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_allMoviesHaveNonZeroPrice));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool anyFreeMovie = await context.Movies.AnyAsync(movie => movie.Price <= 0m);

            Assert.False(anyFreeMovie);
        }

        [Fact]
        public async Task SeedAsync_emptyDatabase_allReelsHaveVideoUrl()
        {
            await using AppDbContext context = CreateContext(nameof(SeedAsync_emptyDatabase_allReelsHaveVideoUrl));

            DataSeeder seeder = new DataSeeder(context);
            await seeder.SeedAsync();

            bool anyMissingUrl = await context.Reels.AnyAsync(reel => reel.VideoUrl == null || reel.VideoUrl == string.Empty);

            Assert.False(anyMissingUrl);
        }
    }
}
