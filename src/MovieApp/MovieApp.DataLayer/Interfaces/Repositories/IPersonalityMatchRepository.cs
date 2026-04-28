using MovieApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Defines the data access contract for the personality match feature, providing methods
    /// to retrieve user preferences, profiles, and movie information required for computing
    /// and displaying personality match results.
    /// </summary>
    public interface IPersonalityMatchRepository
    {
        Task<List<UserMoviePreference>> GetAllPreferencesExceptUserAsync(int excludedUserId);
        Task<List<UserMoviePreference>> GetCurrentUserPreferencesAsync(int userId);
        Task<UserProfile?> GetUserProfileAsync(int userId);
        Task<List<int>> GetRandomUserIdsAsync(int excludedUserId, int userIdsCount);
    }
}

