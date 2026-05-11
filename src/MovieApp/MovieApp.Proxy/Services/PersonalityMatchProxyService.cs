using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Services;

namespace MovieApp.Proxy.Services
{
    /// <summary>
    /// Proxy implementation of IPersonalityMatchService.
    /// Delegates to the real PersonalityMatchService backed by proxy repositories,
    /// so business logic is preserved while data fetching goes over HTTP.
    /// </summary>
    public class PersonalityMatchProxyService : IPersonalityMatchService
    {
        private readonly PersonalityMatchService _inner;

        public PersonalityMatchProxyService(ApiClient apiClient)
        {
            _inner = new PersonalityMatchService(
                new PersonalityMatchProxyRepository(apiClient),
                new UserProxyRepository(apiClient));
        }

        public Task<Dictionary<int, List<UserMoviePreference>>> GetAllPreferencesGroupedAsync(int excludedUserId)
            => _inner.GetAllPreferencesGroupedAsync(excludedUserId);

        public Task<string> GetUsernameAsync(int userId)
            => _inner.GetUsernameAsync(userId);

        public Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount)
            => _inner.GetTopMoviePreferencesAsync(userId, topMoviePreferencesCount);
    }
}
