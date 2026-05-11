using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.PersonalityMatch;

namespace MovieApp.Proxy.Services
{
    /// <summary>
    /// Proxy implementation of IPersonalityMatchingService.
    /// Delegates to the real PersonalityMatchingService backed by PersonalityMatchProxyRepository,
    /// so all cosine-similarity business logic is preserved while data fetching goes over HTTP.
    /// </summary>
    public class PersonalityMatchingProxyService : IPersonalityMatchingService
    {
        private readonly PersonalityMatchingService _inner;

        public PersonalityMatchingProxyService(ApiClient apiClient)
        {
            _inner = new PersonalityMatchingService(new PersonalityMatchProxyRepository(apiClient));
        }

        public Task<List<MatchResult>> GetTopMatchesAsync(int userId, int count)
            => _inner.GetTopMatchesAsync(userId, count);

        public Task<List<MatchResult>> GetRandomUsersAsync(int userId, int count)
            => _inner.GetRandomUsersAsync(userId, count);

        public Task<UserProfile?> GetUserProfileAsync(int userId)
            => _inner.GetUserProfileAsync(userId);

        public Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount)
            => _inner.GetTopMoviePreferencesAsync(userId, topMoviePreferencesCount);

        public Task<string> GetUsernameAsync(int userId)
            => _inner.GetUsernameAsync(userId);
    }
}
