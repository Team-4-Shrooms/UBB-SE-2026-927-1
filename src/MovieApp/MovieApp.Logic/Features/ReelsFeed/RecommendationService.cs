using System.Collections.Generic;
using System.Linq;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.ReelsFeed
{
    /// <summary>
    /// Provides personalized reel recommendations by scoring unwatched reels
    /// against the user's movie preferences, with a cold-start fallback for new users.
    /// Owner: Tudor.
    /// </summary>
    public class RecommendationService : IRecommendationService
    {
        private const int RecentlyLikedDaysWindow = 7;
        private readonly IRecommendationRepository recommendationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationService"/> class.
        /// </summary>
        /// <param name="recommendationRepository">Repository used to load recommendation inputs.</param>
        public RecommendationService(IRecommendationRepository recommendationRepository)
        {
            this.recommendationRepository = recommendationRepository;
        }

        /// <inheritdoc />
        public async Task<IList<Reel>> GetRecommendedReelsAsync(int userId, int count)
        {
            bool userHasPreferences = await this.recommendationRepository.UserHasPreferencesAsync(userId);

            return userHasPreferences
                ? await this.GetPersonalizedReelsAsync(userId, count)
                : await this.GetColdStartReelsAsync(userId, count);
        }

        private async Task<IList<Reel>> GetPersonalizedReelsAsync(int userId, int count)
        {
            IList<Reel> allReels = await this.recommendationRepository.GetAllReelsAsync();
            Dictionary<int, decimal> userPreferenceScores = await this.recommendationRepository.GetUserPreferenceScoresAsync(userId);

            return allReels
                .OrderByDescending(reel =>
                    userPreferenceScores.TryGetValue(reel.Movie.Id, out Decimal preferenceScore) ? preferenceScore : 0)
                .ThenByDescending(reel => reel.CreatedAt)
                .Take(count)
                .ToList();
        }

        private async Task<IList<Reel>> GetColdStartReelsAsync(int userId, int count)
        {
            IList<Reel> allReels = await this.recommendationRepository.GetAllReelsAsync();
            List<UserReelInteraction> recentInteractions = await this.recommendationRepository.GetLikesWithinDaysAsync(RecentlyLikedDaysWindow);

            Dictionary<int, int> recentLikeCountsByReelId = recentInteractions
                .GroupBy(interaction => interaction.Reel.Id) 
                .ToDictionary(group => group.Key, group => group.Count());

            return allReels
                .OrderByDescending(reel =>
                    recentLikeCountsByReelId.TryGetValue(reel.Id, out Int32 recentLikeCount) ? recentLikeCount : 0)
                .ThenByDescending(reel => reel.CreatedAt)
                .Take(count)
                .ToList();
        }

        public async Task<bool> UserHasPreferencesAsync(int userId)
        {
            return await this.recommendationRepository.UserHasPreferencesAsync(userId);
        }

        public async Task<IList<Reel>> GetAllReelsAsync()
        {
            return await this.recommendationRepository.GetAllReelsAsync();
        }

        public async Task<IDictionary<int, decimal>> GetUserPreferenceScoresAsync(int userId)
        {
            return await this.recommendationRepository.GetUserPreferenceScoresAsync(userId);
        }

        public async Task<IDictionary<int, int>> GetAllLikeCountsAsync()
        {
            return await this.recommendationRepository.GetAllLikeCountsAsync();
        }

        public async Task<IList<UserReelInteraction>> GetLikesWithinDaysAsync(int days)
        {
            return await this.recommendationRepository.GetLikesWithinDaysAsync(days);
        }
    }
}
