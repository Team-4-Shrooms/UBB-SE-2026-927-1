using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MovieApp.Tests.Repositories
{
    public class PersonalityMatchRepositoryTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        private static async Task<(User user1, User user2, Movie movie)> SeedTwoUsersAndMovie(AppDbContext context)
        {
            User user1 = new User
            {
                Username = "user1",
                Email = "u1@test.com",
                PasswordHash = "hash",
                Balance = 0m,
            };

            User user2 = new User
            {
                Username = "user2",
                Email = "u2@test.com",
                PasswordHash = "hash",
                Balance = 0m,
            };

            Movie movie = new Movie
            {
                Title = "Test Movie",
                Description = "desc",
                Rating = 8m,
                Price = 10m,
                PrimaryGenre = "Action",
                ReleaseYear = 2020,
                Synopsis = "synopsis",
            };

            context.Users.AddRange(user1, user2);
            context.Movies.Add(movie);
            await context.SaveChangesAsync();

            return (user1, user2, movie);
        }

        [Fact]
        public async Task GetAllPreferencesExceptUserAsync_excludesSpecifiedUser()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetAllPreferencesExceptUserAsync_excludesSpecifiedUser));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 5m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            Dictionary<int, List<UserMoviePreference>> result = await repository.GetAllPreferencesExceptUserAsync(user1.Id);

            Assert.False(result.ContainsKey(user1.Id));
        }

        [Fact]
        public async Task GetAllPreferencesExceptUserAsync_includesOtherUsers()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetAllPreferencesExceptUserAsync_includesOtherUsers));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 5m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            Dictionary<int, List<UserMoviePreference>> result = await repository.GetAllPreferencesExceptUserAsync(user1.Id);

            Assert.True(result.ContainsKey(user2.Id));
        }

        [Fact]
        public async Task GetAllPreferencesExceptUserAsync_noOtherUsers_returnsEmpty()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetAllPreferencesExceptUserAsync_noOtherUsers_returnsEmpty));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            context.UserMoviePreferences.Add(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 5m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            Dictionary<int, List<UserMoviePreference>> result = await repository.GetAllPreferencesExceptUserAsync(user1.Id);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllPreferencesExceptUserAsync_otherUserHasCorrectPreferenceCount()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetAllPreferencesExceptUserAsync_otherUserHasCorrectPreferenceCount));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 5m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            Dictionary<int, List<UserMoviePreference>> result = await repository.GetAllPreferencesExceptUserAsync(user1.Id);

            Assert.Single(result[user2.Id]);
        }

        [Fact]
        public async Task GetCurrentUserPreferencesAsync_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetCurrentUserPreferencesAsync_returnsCorrectCount));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 5m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<UserMoviePreference> result = await repository.GetCurrentUserPreferencesAsync(user1.Id);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetCurrentUserPreferencesAsync_returnsOnlyUserPreferences()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetCurrentUserPreferencesAsync_returnsOnlyUserPreferences));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 5m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<UserMoviePreference> result = await repository.GetCurrentUserPreferencesAsync(user1.Id);

            Assert.Equal(user1.Id, result[0].User.Id);
        }

        [Fact]
        public async Task GetCurrentUserPreferencesAsync_noPreferences_returnsEmpty()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetCurrentUserPreferencesAsync_noPreferences_returnsEmpty));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<UserMoviePreference> result = await repository.GetCurrentUserPreferencesAsync(user1.Id);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserProfileAsync_profileExists_returnsNotNull()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetUserProfileAsync_profileExists_returnsNotNull));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            context.UserProfiles.Add(new UserProfile
            {
                User = user1,
                TotalLikes = 5,
                TotalWatchTimeSeconds = 1000,
                AverageWatchTimeSeconds = 200m,
                TotalClipsViewed = 5,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            UserProfile? result = await repository.GetUserProfileAsync(user1.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetUserProfileAsync_profileExists_returnsCorrectTotalLikes()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetUserProfileAsync_profileExists_returnsCorrectTotalLikes));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            context.UserProfiles.Add(new UserProfile
            {
                User = user1,
                TotalLikes = 5,
                TotalWatchTimeSeconds = 1000,
                AverageWatchTimeSeconds = 200m,
                TotalClipsViewed = 5,
                LikeToViewRatio = 1m,
                LastUpdated = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            UserProfile? result = await repository.GetUserProfileAsync(user1.Id);

            Assert.Equal(5, result!.TotalLikes);
        }

        [Fact]
        public async Task GetUserProfileAsync_noProfile_returnsNull()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetUserProfileAsync_noProfile_returnsNull));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            UserProfile? result = await repository.GetUserProfileAsync(user1.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetRandomUserIdsAsync_multipleUsers_returnsRequestedCount()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetRandomUserIdsAsync_multipleUsers_returnsRequestedCount));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            context.UserMoviePreferences.Add(
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<int> result = await repository.GetRandomUserIdsAsync(user1.Id, 1);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetRandomUserIdsAsync_excludesCurrentUser()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetRandomUserIdsAsync_excludesCurrentUser));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            context.UserMoviePreferences.Add(
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<int> result = await repository.GetRandomUserIdsAsync(user1.Id, 1);

            Assert.DoesNotContain(user1.Id, result);
        }

        [Fact]
        public async Task GetUsernameAsync_userExists_returnsCorrectUsername()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetUsernameAsync_userExists_returnsCorrectUsername));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            string result = await repository.GetUsernameAsync(user1.Id);

            Assert.Equal("user1", result);
        }

        [Fact]
        public async Task GetUsernameAsync_userDoesNotExist_returnsFallback()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetUsernameAsync_userDoesNotExist_returnsFallback));

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            string result = await repository.GetUsernameAsync(999);

            Assert.Equal("User 999", result);
        }

        [Fact]
        public async Task GetTopPreferencesWithTitlesAsync_returnsCorrectCount()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetTopPreferencesWithTitlesAsync_returnsCorrectCount));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            Movie movie2 = new Movie
            {
                Title = "Movie 2",
                Description = "desc",
                Rating = 7m,
                Price = 5m,
                PrimaryGenre = "Drama",
                ReleaseYear = 2021,
                Synopsis = "synopsis",
            };
            context.Movies.Add(movie2);
            await context.SaveChangesAsync();

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 9m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie2,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<MoviePreferenceDisplay> result = await repository.GetTopPreferencesWithTitlesAsync(user1.Id, 2);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetTopPreferencesWithTitlesAsync_firstResultIsBestMovie()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetTopPreferencesWithTitlesAsync_firstResultIsBestMovie));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            Movie movie2 = new Movie
            {
                Title = "Movie 2",
                Description = "desc",
                Rating = 7m,
                Price = 5m,
                PrimaryGenre = "Drama",
                ReleaseYear = 2021,
                Synopsis = "synopsis",
            };
            context.Movies.Add(movie2);
            await context.SaveChangesAsync();

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 9m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie2,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<MoviePreferenceDisplay> result = await repository.GetTopPreferencesWithTitlesAsync(user1.Id, 2);

            Assert.True(result[0].IsBestMovie);
        }

        [Fact]
        public async Task GetTopPreferencesWithTitlesAsync_firstResultHasHighestScore()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetTopPreferencesWithTitlesAsync_firstResultHasHighestScore));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            Movie movie2 = new Movie
            {
                Title = "Movie 2",
                Description = "desc",
                Rating = 7m,
                Price = 5m,
                PrimaryGenre = "Drama",
                ReleaseYear = 2021,
                Synopsis = "synopsis",
            };
            context.Movies.Add(movie2);
            await context.SaveChangesAsync();

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 9m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie2,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<MoviePreferenceDisplay> result = await repository.GetTopPreferencesWithTitlesAsync(user1.Id, 2);

            Assert.Equal(9m, result[0].Score);
        }

        [Fact]
        public async Task GetTopPreferencesWithTitlesAsync_secondResultIsNotBestMovie()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetTopPreferencesWithTitlesAsync_secondResultIsNotBestMovie));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            Movie movie2 = new Movie
            {
                Title = "Movie 2",
                Description = "desc",
                Rating = 7m,
                Price = 5m,
                PrimaryGenre = "Drama",
                ReleaseYear = 2021,
                Synopsis = "synopsis",
            };
            context.Movies.Add(movie2);
            await context.SaveChangesAsync();

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 9m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie2,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<MoviePreferenceDisplay> result = await repository.GetTopPreferencesWithTitlesAsync(user1.Id, 2);

            Assert.False(result[1].IsBestMovie);
        }

        [Fact]
        public async Task GetTopPreferencesWithTitlesAsync_limitedCount_returnsOnlyRequestedCount()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetTopPreferencesWithTitlesAsync_limitedCount_returnsOnlyRequestedCount));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            Movie movie2 = new Movie
            {
                Title = "Movie 2",
                Description = "desc",
                Rating = 7m,
                Price = 5m,
                PrimaryGenre = "Drama",
                ReleaseYear = 2021,
                Synopsis = "synopsis",
            };
            context.Movies.Add(movie2);
            await context.SaveChangesAsync();

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 9m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie2,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<MoviePreferenceDisplay> result = await repository.GetTopPreferencesWithTitlesAsync(user1.Id, 1);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetTopPreferencesWithTitlesAsync_limitedCount_returnsHighestScoringMovie()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetTopPreferencesWithTitlesAsync_limitedCount_returnsHighestScoringMovie));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            Movie movie2 = new Movie
            {
                Title = "Movie 2",
                Description = "desc",
                Rating = 7m,
                Price = 5m,
                PrimaryGenre = "Drama",
                ReleaseYear = 2021,
                Synopsis = "synopsis",
            };
            context.Movies.Add(movie2);
            await context.SaveChangesAsync();

            context.UserMoviePreferences.AddRange(
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie,
                    Score = 9m,
                    LastModified = DateTime.UtcNow,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie2,
                    Score = 3m,
                    LastModified = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<MoviePreferenceDisplay> result = await repository.GetTopPreferencesWithTitlesAsync(user1.Id, 1);

            Assert.Equal(9m, result[0].Score);
        }
    }
}
