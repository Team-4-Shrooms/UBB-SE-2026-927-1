using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MovieApp.Logic.Data
{
    /// <summary>
    /// Provides a single consolidated seed method for the application database.
    /// Replaces the old raw-SQL DatabaseInitializer from the PureCaffeine project.
    /// Call <see cref="SeedAsync"/> once at application startup after running migrations.
    /// </summary>
    public class DataSeeder
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSeeder"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public DataSeeder(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Seeds all required data into the database if it does not already exist.
        /// Safe to call multiple times — all operations are idempotent.
        /// </summary>
        public async Task SeedAsync()
        {
            await SeedUsersAsync();
            await SeedMoviesAsync();
            await SeedMusicTracksAsync();
            await SeedReelsAsync();
            await SeedUserMoviePreferencesAsync();
            await SeedUserProfilesAsync();
            await SeedSellersAsync();
            await SeedActiveSalesAsync();
            await SeedMovieEventsAsync();
            await SeedMovieReviewsAsync();
            await SeedEquipmentAsync();
        }

        private async Task SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                return;
            }

            _context.Users.AddRange(
                new User
                {
                    Username = "User1",
                    Email = "user1@movieapp.com",
                    PasswordHash = "placeholder_hash_1",
                    Balance = 100m,
                },
                new User
                {
                    Username = "Alice",
                    Email = "alice@movieapp.com",
                    PasswordHash = "placeholder_hash_2",
                    Balance = 100m,
                },
                new User
                {
                    Username = "Bob",
                    Email = "bob@movieapp.com",
                    PasswordHash = "placeholder_hash_3",
                    Balance = 100m,
                },
                new User
                {
                    Username = "Carol",
                    Email = "carol@movieapp.com",
                    PasswordHash = "placeholder_hash_4",
                    Balance = 100m,
                },
                new User
                {
                    Username = "Dave",
                    Email = "dave@movieapp.com",
                    PasswordHash = "placeholder_hash_5",
                    Balance = 100m,
                },
                new User
                {
                    Username = "Eve",
                    Email = "eve@movieapp.com",
                    PasswordHash = "placeholder_hash_6",
                    Balance = 100m,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedMoviesAsync()
        {
            if (await _context.Movies.AnyAsync())
            {
                return;
            }

            _context.Movies.AddRange(
                new Movie
                {
                    Title = "Inception",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/vr6ouTojPp0zlSpJvCbODPp19nd.jpg",
                    PrimaryGenre = "Sci-Fi",
                    ReleaseYear = 2010,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Dark Knight",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/a1UL3FTJDgQikYIebnMDhTPFVfm.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2008,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Interstellar",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/wbnrYkn59cdFuu0LNAZ2BWh2i37.jpg",
                    PrimaryGenre = "Adventure",
                    ReleaseYear = 2014,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Matrix",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/p96dm7sCMn4VYAStA6siNz30G1r.jpg",
                    PrimaryGenre = "Sci-Fi",
                    ReleaseYear = 1999,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Parasite",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/7IiTTgloJzvGI1TAYymCfbfl3vT.jpg",
                    PrimaryGenre = "Thriller",
                    ReleaseYear = 2019,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "La La Land",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/uDO8zWDhfWwoFdKS4fzkUJt0Rf0.jpg",
                    PrimaryGenre = "Musical",
                    ReleaseYear = 2016,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Whiplash",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/7fn624j5lj3xTme2SgiLCeuedmO.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 2014,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Grand Budapest Hotel",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/eWdyYQreja6JGCzqHWXpWHDrrPo.jpg",
                    PrimaryGenre = "Comedy",
                    ReleaseYear = 2014,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Avatar",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/gKY6q7SjCkAU6FqvqWybDYgUKIF.jpg",
                    PrimaryGenre = "Sci-Fi",
                    ReleaseYear = 2009,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Titanic",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/9xjZS2rlVxm8SFx8kPC3aIGCOYQ.jpg",
                    PrimaryGenre = "Romance",
                    ReleaseYear = 1997,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Gladiator",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/yafsp1whNDGqmn6vqHdgg0PbZA5.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2000,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Shawshank Redemption",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/tsY4m4IH8MZly4kvxZszbommLKj.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 1994,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Forrest Gump",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/n7NEvy20kMLD0X6lrzoaSGXnr3I.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 1994,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Godfather",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/z4lzwl3Gff5IOWKGiYY7gUFYXUb.jpg",
                    PrimaryGenre = "Crime",
                    ReleaseYear = 1972,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Pulp Fiction",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/vQWk5YBFWF4bZaofAbv0tShwBvQ.jpg",
                    PrimaryGenre = "Crime",
                    ReleaseYear = 1994,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Fight Club",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 1999,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Social Network",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/jD2LsdNu9zWwXjjlbxO0Iibpefz.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 2010,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Joker",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/6zIyfSojJxoCj6mL3TZPFZBByfP.jpg",
                    PrimaryGenre = "Thriller",
                    ReleaseYear = 2019,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Avengers: Endgame",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/5hRf3rT7T5QNTmfbv00yXzpGvXw.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2019,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Iron Man",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/frW3QzyDondJdnd6ydzT8ekKHAw.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2008,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Toy Story",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/RyAWIbSmt4xC859hyQ43wUumM9.jpg",
                    PrimaryGenre = "Animation",
                    ReleaseYear = 1995,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Finding Nemo",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/7YrKAe5GBg2f2WnwMrX9IdLFqCq.jpg",
                    PrimaryGenre = "Animation",
                    ReleaseYear = 2003,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Up",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/f6ytrGR8IJ0qizc2gI0HSJN6OaU.jpg",
                    PrimaryGenre = "Animation",
                    ReleaseYear = 2009,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Lion King",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/Pa9euUkkwUqHJoMh6eIj2XVeV4.jpg",
                    PrimaryGenre = "Animation",
                    ReleaseYear = 1994,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Frozen",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/itAKcobTYGpYT8Phwjd8c9hleTo.jpg",
                    PrimaryGenre = "Animation",
                    ReleaseYear = 2013,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Conjuring",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/A09nKeXAa0FlOr6Y6EVnvAINKQ2.jpg",
                    PrimaryGenre = "Horror",
                    ReleaseYear = 2013,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Get Out",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/yY33WCqDYUHdT6LJWsJUekSM4E.jpg",
                    PrimaryGenre = "Horror",
                    ReleaseYear = 2017,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "A Quiet Place",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/yelqLnp4My4WWqD647wwwpw552P.jpg",
                    PrimaryGenre = "Horror",
                    ReleaseYear = 2018,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Mad Max: Fury Road",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/89BIhHMvGAoxQkniNFB8ENrfzxk.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2015,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "John Wick",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/rP7X52bxOEBc09h5qzHJnHXxE3C.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2014,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Wolf of Wall Street",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/bm6TEyJMpvzTeBOP1V43UfRrrfg.jpg",
                    PrimaryGenre = "Biography",
                    ReleaseYear = 2013,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Django Unchained",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/w1P4HHXJT6CRbcZ2x6Yq2sjWsdF.jpg",
                    PrimaryGenre = "Western",
                    ReleaseYear = 2012,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Revenant",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/6Z6bzPW2K6i5nD2NO8wRZRKQJ5y.jpg",
                    PrimaryGenre = "Adventure",
                    ReleaseYear = 2015,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Blade Runner 2049",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/9pz5bPDufSrJd8yTNjp0apTAVf8.jpg",
                    PrimaryGenre = "Sci-Fi",
                    ReleaseYear = 2017,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Her",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/kBK4UlVOIx6NZyD0QkHlFi9XnAw.jpg",
                    PrimaryGenre = "Romance",
                    ReleaseYear = 2013,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Prestige",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/cNpg2TjWtsut8QUBqezkbHXQFgb.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 2006,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Memento",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/fKTPH2WvH8nHTXeBYBVhawtRqtR.jpg",
                    PrimaryGenre = "Thriller",
                    ReleaseYear = 2000,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Shutter Island",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/qfpFopx4AHd3oTOkj0VGG50AS39.jpg",
                    PrimaryGenre = "Thriller",
                    ReleaseYear = 2010,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedMusicTracksAsync()
        {
            if (await _context.MusicTracks.AnyAsync())
            {
                return;
            }

            _context.MusicTracks.AddRange(
                new MusicTrack
                {
                    TrackName = "Epic Cinematic Theme",
                    Author = "Hans Zimmer",
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3",
                    DurationSeconds = 180.5m,
                },
                new MusicTrack
                {
                    TrackName = "Upbeat Pop Track",
                    Author = "Mark Ronson",
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-2.mp3",
                    DurationSeconds = 150.0m,
                },
                new MusicTrack
                {
                    TrackName = "Dramatic Orchestral",
                    Author = "John Williams",
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-3.mp3",
                    DurationSeconds = 200.3m,
                },
                new MusicTrack
                {
                    TrackName = "Chill Lo-Fi Beats",
                    Author = "Nujabes",
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-4.mp3",
                    DurationSeconds = 120.0m,
                },
                new MusicTrack
                {
                    TrackName = "Action Packed Rock",
                    Author = "AC/DC",
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-5.mp3",
                    DurationSeconds = 165.7m,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedReelsAsync()
        {
            if (await _context.Reels.AnyAsync())
            {
                return;
            }

            User? user1 = await _context.Users.FirstOrDefaultAsync(user => user.Username == "User1");

            if (user1 is null)
            {
                return;
            }

            Movie? inception = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Inception");
            Movie? darkKnight = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Dark Knight");
            Movie? interstellar = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Interstellar");
            Movie? matrix = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Matrix");
            Movie? parasite = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Parasite");
            Movie? laLaLand = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "La La Land");
            Movie? whiplash = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Whiplash");

            if (inception is null || darkKnight is null || interstellar is null ||
                matrix is null || parasite is null || laLaLand is null || whiplash is null)
            {
                return;
            }

            _context.Reels.AddRange(
                new Reel
                {
                    Movie = inception,
                    CreatorUser = user1,
                    VideoUrl = "https://samplelib.com/lib/preview/mp4/sample-10s.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_.jpg",
                    Title = "Inception - Dream Within a Dream",
                    Caption = "Mind-bending scene from Inception where reality bends",
                    FeatureDurationSeconds = 45.5m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                },
                new Reel
                {
                    Movie = inception,
                    CreatorUser = user1,
                    VideoUrl = "https://samplelib.com/lib/preview/mp4/sample-15s.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_.jpg",
                    Title = "Inception - Rotating Hallway Fight",
                    Caption = "The iconic zero-gravity hallway fight sequence",
                    FeatureDurationSeconds = 60.2m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-9),
                },
                new Reel
                {
                    Movie = darkKnight,
                    CreatorUser = user1,
                    VideoUrl = "https://samplelib.com/lib/preview/mp4/sample-20s.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMTMxNTMwODM0NF5BMl5BanBnXkFtZTcwODAyMTk2Mw@@._V1_.jpg",
                    Title = "The Dark Knight - Joker Interrogation",
                    Caption = "Heath Ledger's legendary Joker interrogation scene",
                    FeatureDurationSeconds = 55.0m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                },
                new Reel
                {
                    Movie = darkKnight,
                    CreatorUser = user1,
                    VideoUrl = "https://samplelib.com/lib/preview/mp4/sample-30s.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMTMxNTMwODM0NF5BMl5BanBnXkFtZTcwODAyMTk2Mw@@._V1_.jpg",
                    Title = "The Dark Knight - Batmobile Chase",
                    Caption = "Epic chase scene through Gotham streets",
                    FeatureDurationSeconds = 70.8m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                },
                new Reel
                {
                    Movie = interstellar,
                    CreatorUser = user1,
                    VideoUrl = "https://samplelib.com/lib/preview/mp4/sample-5s.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BZjdkOTU3MDktN2IxOS00OGEyLWFmMjktY2FiMmZkNWIyODZiXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_.jpg",
                    Title = "Interstellar - Docking Scene",
                    Caption = "The intense docking sequence with the spinning station",
                    FeatureDurationSeconds = 90.3m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                },
                new Reel
                {
                    Movie = interstellar,
                    CreatorUser = user1,
                    VideoUrl = "https://filesamples.com/samples/video/mp4/sample_640x360.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BZjdkOTU3MDktN2IxOS00OGEyLWFmMjktY2FiMmZkNWIyODZiXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_.jpg",
                    Title = "Interstellar - Tesseract Scene",
                    Caption = "Cooper in the 5th dimension tesseract",
                    FeatureDurationSeconds = 80.0m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                },
                new Reel
                {
                    Movie = matrix,
                    CreatorUser = user1,
                    VideoUrl = "https://filesamples.com/samples/video/mp4/sample_960x400_ocean_with_audio.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BNzQzOTk3NTAtNDQ2Ny00Njc2LTk3M2QtN2FjYTJjNzQzYzQwXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_.jpg",
                    Title = "The Matrix - Bullet Time",
                    Caption = "The revolutionary bullet time effect scene",
                    FeatureDurationSeconds = 35.5m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                },
                new Reel
                {
                    Movie = parasite,
                    CreatorUser = user1,
                    VideoUrl = "https://test-videos.co.uk/vids/bigbuckbunny/mp4/h264/360/Big_Buck_Bunny_360_10s_1MB.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BYWZjMjk3ZTAtZGYzMC00ODQ0LWI2YTMtYjQ5NDU3N2NmZDIzXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_.jpg",
                    Title = "Parasite - Basement Reveal",
                    Caption = "The shocking basement discovery scene",
                    FeatureDurationSeconds = 50.0m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                },
                new Reel
                {
                    Movie = laLaLand,
                    CreatorUser = user1,
                    VideoUrl = "https://archive.org/download/ElephantsDream/ed_1024_512kb.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMjA2OTYxNTY2Nl5BMl5BanBnXkFtZTgwNzg4OTA5OTE@._V1_.jpg",
                    Title = "La La Land - Highway Opening",
                    Caption = "The colorful highway opening dance number",
                    FeatureDurationSeconds = 65.2m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                },
                new Reel
                {
                    Movie = whiplash,
                    CreatorUser = user1,
                    VideoUrl = "https://media.w3.org/2010/05/sintel/trailer.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMjE4NDYxNTAxNV5BMl5BanBnXkFtZTgwNzM0NDM1MjE@._V1_.jpg",
                    Title = "Whiplash - Final Performance",
                    Caption = "The intense final drum performance",
                    FeatureDurationSeconds = 75.0m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedUserMoviePreferencesAsync()
        {
            if (await _context.UserMoviePreferences.AnyAsync())
            {
                return;
            }

            List<User> users = await _context.Users.OrderBy(user => user.Id).ToListAsync();
            List<Movie> movies = await _context.Movies.OrderBy(movie => movie.Id).ToListAsync();

            if (users.Count < 6 || movies.Count < 8)
            {
                return;
            }

            User user1 = users[0];
            User user2 = users[1];
            User user3 = users[2];
            User user4 = users[3];
            User user5 = users[4];
            User user6 = users[5];

            Movie movie1 = movies[0];
            Movie movie2 = movies[1];
            Movie movie3 = movies[2];
            Movie movie4 = movies[3];
            Movie movie5 = movies[4];
            Movie movie6 = movies[5];
            Movie movie7 = movies[6];
            Movie movie8 = movies[7];

            _context.UserMoviePreferences.AddRange(
                // User 1 — seed for tournament
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie1,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie2,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie3,
                    Score = 7.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie4,
                    Score = 8.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie5,
                    Score = 9.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie6,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie7,
                    Score = 7.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie8,
                    Score = 9.2m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },

                // User 2 (Alice) — very similar to User 1
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie1,
                    Score = 8.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie2,
                    Score = 9.2m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie3,
                    Score = 7.8m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie4,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie5,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie6,
                    Score = 3.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie7,
                    Score = 6.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie8,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },

                // User 3 (Bob) — moderate overlap, prefers dramas/musicals
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie1,
                    Score = 5.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie2,
                    Score = 4.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie5,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie6,
                    Score = 9.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie7,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie8,
                    Score = 8.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },

                // User 4 (Carol) — likes Sci-Fi but differs on drama
                new UserMoviePreference
                {
                    User = user4,
                    Movie = movie1,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user4,
                    Movie = movie3,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user4,
                    Movie = movie4,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user4,
                    Movie = movie6,
                    Score = 2.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user4,
                    Movie = movie7,
                    Score = 3.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },

                // User 5 (Dave) — opposite taste
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie1,
                    Score = 2.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie2,
                    Score = 3.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie3,
                    Score = 2.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie4,
                    Score = 1.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie5,
                    Score = 3.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie6,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie7,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie8,
                    Score = 2.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },

                // User 6 (Eve) — partial overlap
                new UserMoviePreference
                {
                    User = user6,
                    Movie = movie2,
                    Score = 8.8m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user6,
                    Movie = movie5,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user6,
                    Movie = movie8,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedUserProfilesAsync()
        {
            if (await _context.UserProfiles.AnyAsync())
            {
                return;
            }

            List<User> users = await _context.Users.OrderBy(user => user.Id).ToListAsync();

            if (users.Count < 6)
            {
                return;
            }

            _context.UserProfiles.AddRange(
                new UserProfile
                {
                    User = users[0],
                    TotalLikes = 0,
                    TotalWatchTimeSeconds = 0,
                    AverageWatchTimeSeconds = 0m,
                    TotalClipsViewed = 0,
                    LikeToViewRatio = 0m,
                    LastUpdated = DateTime.UtcNow,
                },
                new UserProfile
                {
                    User = users[1],
                    TotalLikes = 42,
                    TotalWatchTimeSeconds = 18000,
                    AverageWatchTimeSeconds = 120.5m,
                    TotalClipsViewed = 150,
                    LikeToViewRatio = 0.28m,
                    LastUpdated = DateTime.UtcNow,
                },
                new UserProfile
                {
                    User = users[2],
                    TotalLikes = 15,
                    TotalWatchTimeSeconds = 7200,
                    AverageWatchTimeSeconds = 90.0m,
                    TotalClipsViewed = 80,
                    LikeToViewRatio = 0.19m,
                    LastUpdated = DateTime.UtcNow,
                },
                new UserProfile
                {
                    User = users[3],
                    TotalLikes = 68,
                    TotalWatchTimeSeconds = 32000,
                    AverageWatchTimeSeconds = 145.0m,
                    TotalClipsViewed = 220,
                    LikeToViewRatio = 0.31m,
                    LastUpdated = DateTime.UtcNow,
                },
                new UserProfile
                {
                    User = users[4],
                    TotalLikes = 8,
                    TotalWatchTimeSeconds = 3600,
                    AverageWatchTimeSeconds = 60.0m,
                    TotalClipsViewed = 60,
                    LikeToViewRatio = 0.13m,
                    LastUpdated = DateTime.UtcNow,
                },
                new UserProfile
                {
                    User = users[5],
                    TotalLikes = 25,
                    TotalWatchTimeSeconds = 12000,
                    AverageWatchTimeSeconds = 110.0m,
                    TotalClipsViewed = 109,
                    LikeToViewRatio = 0.23m,
                    LastUpdated = DateTime.UtcNow,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedSellersAsync()
        {
            bool dummy1Exists = await _context.Users.AnyAsync(user => user.Username == "dummy1");
            bool dummy2Exists = await _context.Users.AnyAsync(user => user.Username == "dummy2");

            if (dummy1Exists && dummy2Exists)
            {
                return;
            }

            if (!dummy1Exists)
            {
                _context.Users.Add(new User
                {
                    Username = "dummy1",
                    Email = "dummy1@gmail.com",
                    PasswordHash = "pass1",
                    Balance = 0m,
                });
            }

            if (!dummy2Exists)
            {
                _context.Users.Add(new User
                {
                    Username = "dummy2",
                    Email = "dummy2@gmail.com",
                    PasswordHash = "pass2",
                    Balance = 50m,
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedActiveSalesAsync()
        {
            if (await _context.ActiveSales.AnyAsync())
            {
                return;
            }

            Movie? inception = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Inception");
            Movie? matrix = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Matrix");
            Movie? interstellar = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Interstellar");

            if (inception is null || matrix is null || interstellar is null)
            {
                return;
            }

            DateTime currentDate = DateTime.UtcNow;

            _context.ActiveSales.AddRange(
                new ActiveSale
                {
                    Movie = inception,
                    DiscountPercentage = 20.00m,
                    StartTime = currentDate.AddDays(-1),
                    EndTime = currentDate.AddDays(5),
                },
                new ActiveSale
                {
                    Movie = matrix,
                    DiscountPercentage = 20.00m,
                    StartTime = currentDate.AddDays(-1),
                    EndTime = currentDate.AddDays(5),
                },
                new ActiveSale
                {
                    Movie = interstellar,
                    DiscountPercentage = 35.00m,
                    StartTime = currentDate.AddDays(-1),
                    EndTime = currentDate.AddDays(5),
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedMovieEventsAsync()
        {
            if (await _context.MovieEvents.AnyAsync())
            {
                return;
            }

            Movie? inception = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Inception");
            Movie? matrix = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Matrix");
            Movie? interstellar = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Interstellar");
            Movie? whiplash = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Whiplash");

            if (inception is null || matrix is null || interstellar is null || whiplash is null)
            {
                return;
            }

            DateTime currentDate = DateTime.UtcNow;

            _context.MovieEvents.AddRange(
                new MovieEvent
                {
                    Movie = inception,
                    Title = "Inception - Midnight Screening",
                    Description = "One-night-only midnight screening with a short pre-show talk.",
                    Date = currentDate.AddDays(7),
                    Location = "Cinema Hall A",
                    TicketPrice = 12.50m,
                    PosterUrl = "https://m.media-amazon.com/images/I/71DwIcSgFcS._AC_UF894,1000_QL80_.jpg",
                },
                new MovieEvent
                {
                    Movie = matrix,
                    Title = "The Matrix - Fan Marathon",
                    Description = "Back-to-back screening + trivia. Doors open 18:00.",
                    Date = currentDate.AddDays(14),
                    Location = "Retro Theater",
                    TicketPrice = 18.00m,
                    PosterUrl = "https://m.media-amazon.com/images/I/51EG732BV3L.jpg",
                },
                new MovieEvent
                {
                    Movie = interstellar,
                    Title = "Interstellar - Space Night",
                    Description = "Screening followed by a small astronomy Q&A.",
                    Date = currentDate.AddDays(21),
                    Location = "Science Center Auditorium",
                    TicketPrice = 15.00m,
                    PosterUrl = "https://m.media-amazon.com/images/I/91vIHsL-zjL._AC_UF894,1000_QL80_.jpg",
                },
                new MovieEvent
                {
                    Movie = whiplash,
                    Title = "Whiplash - Live Jazz Intro",
                    Description = "Short live jazz set before the movie.",
                    Date = currentDate.AddDays(10),
                    Location = "Downtown Arts Cinema",
                    TicketPrice = 14.00m,
                    PosterUrl = "https://m.media-amazon.com/images/I/81hKZ6oTqUL._AC_UF894,1000_QL80_.jpg",
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedMovieReviewsAsync()
        {
            if (await _context.MovieReviews.AnyAsync())
            {
                return;
            }

            User? seller1 = await _context.Users.FirstOrDefaultAsync(user => user.Username == "dummy1");
            User? seller2 = await _context.Users.FirstOrDefaultAsync(user => user.Username == "dummy2");

            if (seller1 is null || seller2 is null)
            {
                return;
            }

            Movie? matrix = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Matrix");
            Movie? interstellar = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Interstellar");
            Movie? parasite = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Parasite");
            Movie? johnWick = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "John Wick");
            Movie? whiplash = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Whiplash");

            if (matrix is null || interstellar is null || parasite is null || johnWick is null || whiplash is null)
            {
                return;
            }

            DateTime currentDate = DateTime.UtcNow;

            _context.MovieReviews.AddRange(
                new MovieReview
                {
                    Movie = matrix,
                    User = seller1,
                    StarRating = 9m,
                    Comment = "A mind-bending classic with unforgettable world-building.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = matrix,
                    User = seller2,
                    StarRating = 7m,
                    Comment = "Great action and ideas, but definitely not for everyone.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = interstellar,
                    User = seller1,
                    StarRating = 10m,
                    Comment = "Epic, emotional, and incredibly thought-provoking.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = interstellar,
                    User = seller2,
                    StarRating = 8m,
                    Comment = "Beautiful visuals and a satisfying emotional payoff.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = parasite,
                    User = seller1,
                    StarRating = 9m,
                    Comment = "Smart, tense, and darkly funny all the way through.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = parasite,
                    User = seller2,
                    StarRating = 6m,
                    Comment = "Surprisingly entertaining, but the pacing felt uneven.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = johnWick,
                    User = seller1,
                    StarRating = 8m,
                    Comment = "Non-stop style and killer action choreography.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = johnWick,
                    User = seller2,
                    StarRating = 7m,
                    Comment = "Solid thrills and great atmosphere; easy to binge.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = whiplash,
                    User = seller1,
                    StarRating = 9m,
                    Comment = "A brutal, addictive rivalry that stays with you.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = whiplash,
                    User = seller2,
                    StarRating = 8m,
                    Comment = "Fantastic performances and a soundtrack that demands attention.",
                    CreatedAt = currentDate,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedEquipmentAsync()
        {
            if (await _context.Equipment.AnyAsync())
            {
                return;
            }

            User? seller1 = await _context.Users.FirstOrDefaultAsync(user => user.Username == "dummy1");
            User? seller2 = await _context.Users.FirstOrDefaultAsync(user => user.Username == "dummy2");

            if (seller1 is null || seller2 is null)
            {
                return;
            }

            _context.Equipment.AddRange(
                new Equipment
                {
                    Seller = seller1,
                    Title = "Canon EOS 2000D Kit",
                    Category = "Cameras",
                    Description = "24.1 MP APS-C CMOS sensor. Perfect entry-level DSLR for student films, includes 18-55mm IS II Lens and 1080p cinematic video mode.",
                    Condition = "Good",
                    Price = 1200.00m,
                    ImageUrl = "https://static0.pocketlintimages.com/wordpress/wp-content/uploads/wm/143700-cameras-review-hands-on-canon-eos-2000d-review-image1-xploy5pbva.jpg",
                    Status = EquipmentStatus.Available,
                },
                new Equipment
                {
                    Seller = seller1,
                    Title = "Rode NTG Shotgun Mic",
                    Category = "Audio",
                    Description = "Professional directional condenser microphone. Super-cardioid polar pattern, ideal for isolating dialogue on noisy film sets.",
                    Condition = "New",
                    Price = 1200.00m,
                    ImageUrl = "https://fstudio.vtexassets.com/arquivos/ids/750303-1200-1200",
                    Status = EquipmentStatus.Sold,
                },
                new Equipment
                {
                    Seller = seller2,
                    Title = "Blackmagic Pocket Cinema 6K",
                    Category = "Cameras",
                    Description = "EF Mount, Super 35 HDR sensor, 13 stops of dynamic range and dual native ISO up to 25,600 for incredible low light performance.",
                    Condition = "Like New",
                    Price = 9500.00m,
                    ImageUrl = "https://images.blackmagicdesign.com/images/products/blackmagicpocketcinemacamera/main/pocket-6k-g2-xl.jpg",
                    Status = EquipmentStatus.Available,
                },
                new Equipment
                {
                    Seller = seller1,
                    Title = "DJI RS 3 Pro Gimbal",
                    Category = "Stabilization",
                    Description = "Carbon fiber construction, 4.5kg (10lbs) tested payload. Automated axis locks and LiDAR focusing for professional solo cinematographers.",
                    Condition = "New",
                    Price = 4200.00m,
                    ImageUrl = "https://m.media-amazon.com/images/I/61S6h1S-z3L._AC_SL1500_.jpg",
                    Status = EquipmentStatus.Available,
                },
                new Equipment
                {
                    Seller = seller2,
                    Title = "Atomos Ninja V+ Monitor",
                    Category = "Monitoring",
                    Description = "5-inch 4K HDMI Recording Monitor. 1000 nits brightness for outdoor use, supports ProRes RAW recording directly from camera sensor.",
                    Condition = "Used",
                    Price = 2800.00m,
                    ImageUrl = "https://m.media-amazon.com/images/I/71N-W-vV6NL._AC_SL1500_.jpg",
                    Status = EquipmentStatus.Available,
                });

            await _context.SaveChangesAsync();
        }
    }
}
