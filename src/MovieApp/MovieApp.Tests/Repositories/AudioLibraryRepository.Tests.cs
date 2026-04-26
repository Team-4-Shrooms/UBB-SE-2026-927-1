using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.Tests.Repositories
{
    public class AudioLibraryRepositoryTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetAllTracksAsync_tracksExist_returnsAllTracksOrderedByName()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllTracksAsync_tracksExist_returnsAllTracksOrderedByName));

            context.MusicTracks.AddRange(
                new MusicTrack { TrackName = "Zebra Song", Author = "Artist A", AudioUrl = "url1", DurationSeconds = 180m },
                new MusicTrack { TrackName = "Alpha Song", Author = "Artist B", AudioUrl = "url2", DurationSeconds = 200m });
            await context.SaveChangesAsync();

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            IList<MusicTrack> result = await repository.GetAllTracksAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("Alpha Song", result[0].TrackName);
        }

        [Fact]
        public async Task GetAllTracksAsync_noTracks_returnsEmpty()
        {
            await using AppDbContext context = CreateContext(nameof(GetAllTracksAsync_noTracks_returnsEmpty));

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            IList<MusicTrack> result = await repository.GetAllTracksAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTrackByIdAsync_trackExists_returnsTrack()
        {
            await using AppDbContext context = CreateContext(nameof(GetTrackByIdAsync_trackExists_returnsTrack));

            MusicTrack track = new MusicTrack
            {
                TrackName = "Test Track",
                Author = "Test Artist",
                AudioUrl = "http://audio.url",
                DurationSeconds = 180m,
            };

            context.MusicTracks.Add(track);
            await context.SaveChangesAsync();

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            MusicTrack? result = await repository.GetTrackByIdAsync(track.Id);

            Assert.NotNull(result);
            Assert.Equal("Test Track", result.TrackName);
        }

        [Fact]
        public async Task GetTrackByIdAsync_trackDoesNotExist_returnsNull()
        {
            await using AppDbContext context = CreateContext(nameof(GetTrackByIdAsync_trackDoesNotExist_returnsNull));

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            MusicTrack? result = await repository.GetTrackByIdAsync(999);

            Assert.Null(result);
        }
    }
}
