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
        public async Task GetAllPreferencesExceptUserAsync_TwoUsersWithPreferences_ExcludesSpecifiedUser()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetAllPreferencesExceptUserAsync_TwoUsersWithPreferences_ExcludesSpecifiedUser));
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

            List<UserMoviePreference> otherPreferences = await repository.GetAllPreferencesExceptUserAsync(user1.Id);

            bool excludesSpecifiedUser = otherPreferences.All(preference => preference.User.Id != user1.Id);

            Assert.True(excludesSpecifiedUser);
        }

        [Fact]
        public async Task GetAllPreferencesExceptUserAsync_TwoUsersWithPreferences_IncludesOtherUserPreferences()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetAllPreferencesExceptUserAsync_TwoUsersWithPreferences_IncludesOtherUserPreferences));
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

            List<UserMoviePreference> otherPreferences = await repository.GetAllPreferencesExceptUserAsync(user1.Id);

            bool containsOtherUser = otherPreferences.Any(preference => preference.User.Id == user2.Id);

            Assert.True(containsOtherUser);
        }

        [Fact]
        public async Task GetAllPreferencesExceptUserAsync_OnlyExcludedUserHasPreferences_ReturnsEmptyList()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetAllPreferencesExceptUserAsync_OnlyExcludedUserHasPreferences_ReturnsEmptyList));
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

            List<UserMoviePreference> otherPreferences = await repository.GetAllPreferencesExceptUserAsync(user1.Id);

            Assert.Empty(otherPreferences);
        }

        [Fact]
        public async Task GetAllPreferencesExceptUserAsync_OtherUserHasOnePreference_ReturnsSinglePreference()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetAllPreferencesExceptUserAsync_OtherUserHasOnePreference_ReturnsSinglePreference));
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

            List<UserMoviePreference> otherPreferences = await repository.GetAllPreferencesExceptUserAsync(user1.Id);

            Assert.Single(otherPreferences);
        }

        [Fact]
        public async Task GetCurrentUserPreferencesAsync_UserHasOnePreference_ReturnsSinglePreference()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetCurrentUserPreferencesAsync_UserHasOnePreference_ReturnsSinglePreference));
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

            List<UserMoviePreference> userPreferences = await repository.GetCurrentUserPreferencesAsync(user1.Id);

            Assert.Single(userPreferences);
        }

        [Fact]
        public async Task GetCurrentUserPreferencesAsync_TwoUsersWithPreferences_ReturnsOnlyRequestedUserPreferences()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetCurrentUserPreferencesAsync_TwoUsersWithPreferences_ReturnsOnlyRequestedUserPreferences));
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

            List<UserMoviePreference> userPreferences = await repository.GetCurrentUserPreferencesAsync(user1.Id);

            Assert.Equal(user1.Id, userPreferences[0].User.Id);
        }

        [Fact]
        public async Task GetCurrentUserPreferencesAsync_UserHasNoPreferences_ReturnsEmptyList()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetCurrentUserPreferencesAsync_UserHasNoPreferences_ReturnsEmptyList));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            List<UserMoviePreference> userPreferences = await repository.GetCurrentUserPreferencesAsync(user1.Id);

            Assert.Empty(userPreferences);
        }

        [Fact]
        public async Task GetUserProfileAsync_ProfileExists_ReturnsNotNull()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetUserProfileAsync_ProfileExists_ReturnsNotNull));
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

            UserProfile? foundProfile = await repository.GetUserProfileAsync(user1.Id);

            Assert.NotNull(foundProfile);
        }

        [Fact]
        public async Task GetUserProfileAsync_ProfileExists_ReturnsCorrectTotalLikes()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetUserProfileAsync_ProfileExists_ReturnsCorrectTotalLikes));
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

            UserProfile? foundProfile = await repository.GetUserProfileAsync(user1.Id);

            Assert.Equal(5, foundProfile!.TotalLikes);
        }

        [Fact]
        public async Task GetUserProfileAsync_ProfileDoesNotExist_ReturnsNull()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetUserProfileAsync_ProfileDoesNotExist_ReturnsNull));
            (User user1, User user2, Movie movie) = await SeedTwoUsersAndMovie(context);

            PersonalityMatchRepository repository = new PersonalityMatchRepository(context);

            UserProfile? foundProfile = await repository.GetUserProfileAsync(user1.Id);

            Assert.Null(foundProfile);
        }

        [Fact]
        public async Task GetRandomUserIdsAsync_OtherUserHasPreferences_ReturnsSingleUserId()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetRandomUserIdsAsync_OtherUserHasPreferences_ReturnsSingleUserId));
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

            List<int> randomUserIds = await repository.GetRandomUserIdsAsync(user1.Id, 1);

            Assert.Single(randomUserIds);
        }

        [Fact]
        public async Task GetRandomUserIdsAsync_OtherUserHasPreferences_ExcludesCurrentUser()
        {
            await using AppDbContext context = CreateContext("Personality_" + nameof(GetRandomUserIdsAsync_OtherUserHasPreferences_ExcludesCurrentUser));
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

            List<int> randomUserIds = await repository.GetRandomUserIdsAsync(user1.Id, 1);

            Assert.DoesNotContain(user1.Id, randomUserIds);
        }
    }
}
