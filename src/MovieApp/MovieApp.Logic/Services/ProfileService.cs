using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepo;
        private readonly IUserRepository _userRepo;

        public ProfileService(IProfileRepository profileRepo, IUserRepository userRepo)
        {
            _profileRepo = profileRepo;
            _userRepo = userRepo;
        }

        public async Task<UserProfile> BuildProfileFromInteractionsAsync(int userId)
        {
            User user = await _userRepo.GetUserByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            List<UserReelInteraction> interactions = await _profileRepo.GetInteractionsAsync(userId);

            int likes = interactions.Count(i => i.IsLiked);
            int views = interactions.Count;
            long seconds = (long)interactions.Sum(i => i.WatchDurationSeconds);

            return new UserProfile
            {
                User = user,
                TotalLikes = likes,
                TotalClipsViewed = views,
                TotalWatchTimeSeconds = seconds,
                LikeToViewRatio = views == 0 ? 0m : (decimal)likes / views,
                AverageWatchTimeSeconds = views == 0 ? 0m : (decimal)seconds / views,
                LastUpdated = DateTime.UtcNow
            };
        }
    }
}

