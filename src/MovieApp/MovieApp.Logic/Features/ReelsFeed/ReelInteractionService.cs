using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;

namespace MovieApp.Logic.Features.ReelsFeed
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
            UserReelInteraction existingInteraction = await this.interactionRepository.GetInteractionAsync(userId, reelId);
            bool wasLiked = existingInteraction?.IsLiked ?? false;

            await this.interactionRepository.ToggleLikeAsync(userId, reelId);

            if (!wasLiked)
            {
                int? associatedMovieId = await this.interactionRepository.GetReelMovieIdAsync(reelId);
                if (associatedMovieId.HasValue)
                {
                    await this.BoostPreferenceOnLikeAsync(userId, associatedMovieId.Value);
                }
            }
        }

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
