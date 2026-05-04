using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;

namespace MovieApp.Logic.Features.MovieSwipe
{
    /// <summary>
    /// Implements swipe processing using defined score constants.
    /// </summary>
    public class SwipeService : ISwipeService
    {
        public const double LikeDelta = 1.0;
        public const double SkipDelta = -0.5;
        private const int LikedIndicator = 1;
        private const int SkippedIndicator = -1;

        private readonly IPreferenceRepository preferenceRepository;

        public SwipeService(IPreferenceRepository preferenceRepository)
        {
            this.preferenceRepository = preferenceRepository;
        }

        public async Task UpdatePreferenceScoreAsync(int userId, int movieId, bool isLiked)
        {
            double delta = isLiked ? LikeDelta : SkipDelta;

            UserMoviePreference preference = new UserMoviePreference
            {
                User = new User { Id = userId },
                Movie = new Movie { Id = movieId },
                Score = (decimal)delta,
                LastModified = DateTime.UtcNow,
                ChangeFromPreviousValue = isLiked ? LikedIndicator : SkippedIndicator
            };

            if (await preferenceRepository.PreferenceExistsAsync(userId, movieId))
            {
                await preferenceRepository.UpdatePreferenceAsync(userId, movieId, (decimal)delta);
            }
            else
            {
                await preferenceRepository.InsertPreferenceAsync(userId, movieId, (decimal)delta);
            }
        }
    }
}
