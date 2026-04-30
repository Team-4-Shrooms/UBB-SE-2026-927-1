using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.PersonalityMatch
{
    /// <summary>
    /// Defines the contract for the personality matching service.
    /// </summary>
    public interface IPersonalityMatchingService
    {
        Task<List<MatchResult>> GetTopMatchesAsync(int userId, int count);
        Task<List<MatchResult>> GetRandomUsersAsync(int userId, int count);
        Task<UserProfile?> GetUserProfileAsync(int userId);
        Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount);
        Task<string> GetUsernameAsync(int userId);
    }
}
