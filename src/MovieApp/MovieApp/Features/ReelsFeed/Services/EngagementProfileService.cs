using MovieApp.DataLayer.Models;


namespace MovieApp.Features.ReelsFeed.Services
{
    /// <summary>
    /// Computes the user's engagement profile by aggregating raw interaction data,
    /// then delegates persistence to <see cref="IProfileRepository"/>.
    /// Owner: Tudor.
    /// </summary>
    public class EngagementProfileService : IEngagementProfileService
    {
        private readonly IProfileRepository profileRepository;

        public EngagementProfileService(IProfileRepository profileRepository)
        {
            this.profileRepository = profileRepository;
        }

        /// <inheritdoc />
        public async Task<UserProfile?> GetProfileAsync(int userId)
        {
            return await this.profileRepository.GetProfileAsync(userId);
        }

        /// <inheritdoc />
        /// <inheritdoc />
        public async Task RefreshProfileAsync(int userId)
        {
            System.Collections.Generic.List<UserReelInteraction> interactions = await this.profileRepository.GetInteractionsAsync(userId);

            int totalLikes = System.Linq.Enumerable.Count(interactions, i => i.IsLiked);
            decimal totalWatchTime = System.Linq.Enumerable.Sum(interactions, i => i.WatchDurationSeconds);
            int totalClipsViewed = interactions.Count;

            decimal avgWatchTime = totalClipsViewed > 0 ? totalWatchTime / totalClipsViewed : 0m;
            double likeToViewRatio = totalClipsViewed > 0 ? (double)totalLikes / totalClipsViewed : 0.0;

            UserProfile? profile = await this.profileRepository.GetProfileAsync(userId);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    User = new User { Id = userId },
                    TotalLikes = totalLikes,
                    TotalWatchTimeSeconds = (long)totalWatchTime,
                    AverageWatchTimeSeconds = avgWatchTime,
                    TotalClipsViewed = totalClipsViewed,
                    LikeToViewRatio = (long)likeToViewRatio
                };
                await this.profileRepository.AddProfileAsync(profile);
            }
            else
            {
                profile.TotalLikes = totalLikes;
                profile.TotalWatchTimeSeconds = (long)totalWatchTime;
                profile.AverageWatchTimeSeconds = avgWatchTime;
                profile.TotalClipsViewed = totalClipsViewed;
                profile.LikeToViewRatio = (long)likeToViewRatio;
            }

            await this.profileRepository.SaveChangesAsync();
        }
    }
}
