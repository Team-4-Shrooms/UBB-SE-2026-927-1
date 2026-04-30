using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Services
{
    public class PersonalityMatchService : IPersonalityMatchService
    {
        private const string FallbackUsernamePrefix = "User";
        private readonly IPersonalityMatchRepository _matchRepo;
        private readonly IUserRepository _userRepo;

        public PersonalityMatchService(IPersonalityMatchRepository matchRepo, IUserRepository userRepo)
        {
            _matchRepo = matchRepo;
            _userRepo = userRepo;
        }

        public async Task<Dictionary<int, List<UserMoviePreference>>> GetAllPreferencesGroupedAsync(int excludedUserId)
        {
            var flatList = await _matchRepo.GetAllPreferencesExceptUserAsync(excludedUserId);

            return flatList
                .GroupBy(p => p.User.Id)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public async Task<string> GetUsernameAsync(int userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            return user?.Username ?? $"{FallbackUsernamePrefix} {userId}";
        }

        public async Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int count)
        {
            var preferences = await _matchRepo.GetCurrentUserPreferencesAsync(userId);

            var topPreferences = preferences
                .OrderByDescending(p => p.Score)
                .Take(count)
                .ToList();

            var displayModels = new List<MoviePreferenceDisplay>();
            bool isFirst = true;

            foreach (var preference in topPreferences)
            {
                displayModels.Add(new MoviePreferenceDisplay
                {
                    MovieId = preference.Movie.Id,
                    Title = preference.Movie.Title,
                    Score = preference.Score,
                    IsBestMovie = isFirst
                });
                isFirst = false;
            }
            return displayModels;
        }
    }
}

