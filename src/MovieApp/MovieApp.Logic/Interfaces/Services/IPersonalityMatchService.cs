using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IPersonalityMatchService
    {
        Task<Dictionary<int, List<UserMoviePreference>>> GetAllPreferencesGroupedAsync(int excludedUserId);
        Task<string> GetUsernameAsync(int userId);
        Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount);
        Task<List<UserMoviePreference>> GetCurrentUserPreferencesAsync(int userId);
        Task<List<int>> GetRandomUserIdsAsync(int excludedUserId, int userIdsCount);
    }
}

