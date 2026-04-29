using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
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
        public async Task GetAllTracksAsync_tracksExist_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetAllTracksAsync_tracksExist_returnsCorrectCount));

            context.MusicTracks.AddRange(
                new MusicTrack
                {
                    TrackName = "Zebra Song",
                    Author = "Artist A",
                    AudioUrl = "url1",
                    DurationSeconds = 180m,
                },
                new MusicTrack
                {
                    TrackName = "Alpha Song",
                    Author = "Artist B",
                    AudioUrl = "url2",
                    DurationSeconds = 200m,
                });
            await context.SaveChangesAsync();

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            IList<MusicTrack> result = await repository.GetAllTracksAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllTracksAsync_tracksExist_returnsTracksOrderedByName()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetAllTracksAsync_tracksExist_returnsTracksOrderedByName));

            context.MusicTracks.AddRange(
                new MusicTrack
                {
                    TrackName = "Zebra Song",
                    Author = "Artist A",
                    AudioUrl = "url1",
                    DurationSeconds = 180m,
                },
                new MusicTrack
                {
                    TrackName = "Alpha Song",
                    Author = "Artist B",
                    AudioUrl = "url2",
                    DurationSeconds = 200m,
                });
            await context.SaveChangesAsync();

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            IList<MusicTrack> result = await repository.GetAllTracksAsync();

            Assert.Equal("Alpha Song", result[0].TrackName);
        }

        [Fact]
        public async Task GetAllTracksAsync_tracksExist_lastTrackIsCorrect()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetAllTracksAsync_tracksExist_lastTrackIsCorrect));

            context.MusicTracks.AddRange(
                new MusicTrack
                {
                    TrackName = "Zebra Song",
                    Author = "Artist A",
                    AudioUrl = "url1",
                    DurationSeconds = 180m,
                },
                new MusicTrack
                {
                    TrackName = "Alpha Song",
                    Author = "Artist B",
                    AudioUrl = "url2",
                    DurationSeconds = 200m,
                });
            await context.SaveChangesAsync();

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            IList<MusicTrack> result = await repository.GetAllTracksAsync();

            Assert.Equal("Zebra Song", result[1].TrackName);
        }

        [Fact]
        public async Task GetAllTracksAsync_noTracks_returnsEmpty()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetAllTracksAsync_noTracks_returnsEmpty));

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            IList<MusicTrack> result = await repository.GetAllTracksAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTrackByIdAsync_trackExists_returnsNotNull()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetTrackByIdAsync_trackExists_returnsNotNull));

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
        }

        [Fact]
        public async Task GetTrackByIdAsync_trackExists_returnsCorrectTrackName()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetTrackByIdAsync_trackExists_returnsCorrectTrackName));

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

            Assert.Equal("Test Track", result!.TrackName);
        }

        [Fact]
        public async Task GetTrackByIdAsync_trackExists_returnsCorrectAuthor()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetTrackByIdAsync_trackExists_returnsCorrectAuthor));

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

            Assert.Equal("Test Artist", result!.Author);
        }

        [Fact]
        public async Task GetTrackByIdAsync_trackExists_returnsCorrectDuration()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetTrackByIdAsync_trackExists_returnsCorrectDuration));

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

            Assert.Equal(180m, result!.DurationSeconds);
        }

        [Fact]
        public async Task GetTrackByIdAsync_trackDoesNotExist_returnsNull()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetTrackByIdAsync_trackDoesNotExist_returnsNull));

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            MusicTrack? result = await repository.GetTrackByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllTracksAsync_singleTrack_returnsOneItem()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetAllTracksAsync_singleTrack_returnsOneItem));

            context.MusicTracks.Add(new MusicTrack
            {
                TrackName = "Solo Track",
                Author = "Solo Artist",
                AudioUrl = "url1",
                DurationSeconds = 90m,
            });
            await context.SaveChangesAsync();

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            IList<MusicTrack> result = await repository.GetAllTracksAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetTrackByIdAsync_wrongId_returnsNull()
        {
            await using AppDbContext context = CreateContext("Audio_" + nameof(GetTrackByIdAsync_wrongId_returnsNull));

            context.MusicTracks.Add(new MusicTrack
            {
                TrackName = "Some Track",
                Author = "Some Artist",
                AudioUrl = "url1",
                DurationSeconds = 120m,
            });
            await context.SaveChangesAsync();

            AudioLibraryRepository repository = new AudioLibraryRepository(context);

            MusicTrack? result = await repository.GetTrackByIdAsync(999);

            Assert.Null(result);
        }
    }
}
