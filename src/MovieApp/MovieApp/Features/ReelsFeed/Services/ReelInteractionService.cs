using MovieApp.Logic.Models;


namespace MovieApp.Features.ReelsFeed.Services
{
    /// <summary>
    /// Orchestrates user–reel interactions by delegating persistence to
    /// <see cref="IInteractionRepository"/> and preference boosts to
    /// <see cref="IPreferenceRepository"/>.
    /// Owner: Tudor.
    /// </summary>
    public class ReelInteractionService : IReelInteractionService
    {
        private const double LikeBoostAmount = 1.5;
        private readonly IInteractionRepository interactionRepository;
        private readonly IPreferenceRepository preferenceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReelInteractionService"/> class.
        /// </summary>
        /// <param name="interactionRepository">Repository used for reel interaction persistence.</param>
        /// <param name="preferenceRepository">Repository used for updating movie preference boosts.</param>
        public ReelInteractionService(
            IInteractionRepository interactionRepository,
            IPreferenceRepository preferenceRepository)
        {
            this.interactionRepository = interactionRepository;
            this.preferenceRepository = preferenceRepository;
        }

        /// <inheritdoc />
        public async Task ToggleLikeAsync(int userId, int reelId)
        {
            // Implementation detail: apply a preference boost only on unliked -> liked transitions.
            UserReelInteraction existingInteraction = await this.interactionRepository.GetInteractionAsync(userId, reelId);
            bool wasLiked = existingInteraction?.IsLiked ?? false;

            await this.interactionRepository.ToggleLikeAsync(userId, reelId);

            // Boost preference only when transitioning from unliked → liked
            if (!wasLiked)
            {
                int? associatedMovieId = await this.interactionRepository.GetReelMovieIdAsync(reelId);
                if (associatedMovieId.HasValue)
                {
                    await this.BoostPreferenceOnLikeAsync(userId, associatedMovieId.Value);
                }
            }
        }

        /// <summary>
        /// Boosts user's preference for a movie by applying upsert logic:
        /// if preference doesn't exist, insert with boost amount;  otherwise, add boost amount.
        /// </summary>
        private async Task BoostPreferenceOnLikeAsync(int userId, int movieId)
        {
            Boolean preferenceExists = await this.preferenceRepository.PreferenceExistsAsync(userId, movieId);

            if (!preferenceExists)
            {
                await this.preferenceRepository.InsertPreferenceAsync(userId, movieId, (decimal)LikeBoostAmount);
            }
            else
            {
                await this.preferenceRepository.UpdatePreferenceAsync(userId, movieId, (decimal)LikeBoostAmount);
            }
        }


        /// <inheritdoc />
        public async Task RecordViewAsync(int userId, int reelId, double watchDurationSec, double watchPercentage)
        {
            await this.interactionRepository.UpdateViewDataAsync(
                userId,
                reelId,
                (decimal)watchDurationSec,
                (decimal)watchPercentage);
        }


        /// <inheritdoc />
        public async Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId)
        {
            return await this.interactionRepository.GetInteractionAsync(userId, reelId);
        }

        /// <inheritdoc />
        public async Task<int> GetLikeCountAsync(int reelId)
        {
            return await this.interactionRepository.GetLikeCountAsync(reelId);
        }
    }
}
