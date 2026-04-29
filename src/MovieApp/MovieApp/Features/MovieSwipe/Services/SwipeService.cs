using MovieApp.DataLayer.Models;

namespace MovieApp.Features.MovieSwipe.Services
{
    /// <summary>
    /// Implements swipe processing using defined score constants.
    /// </summary>
    public class SwipeService : ISwipeService
    {
        /// <summary> The score increase for a positive swipe. </summary>
        public const double LikeDelta = 1.0;

        /// <summary> The score decrease for a negative swipe. </summary>
        public const double SkipDelta = -0.5;

        /// <summary> Indicator for a positive change value. </summary>
        private const int LikedIndicator = 1;

        /// <summary> Indicator for a negative change value. </summary>
        private const int SkippedIndicator = -1;

        /// <summary> The preference repository for data persistence. </summary>
        private readonly IPreferenceRepository preferenceRepository;

        /// <summary> Initializes a new instance of the <see cref="SwipeService"/> class. </summary>
        /// <param name="preferenceRepository">The preference repository.</param>
        public SwipeService(IPreferenceRepository preferenceRepository)
        {
            this.preferenceRepository = preferenceRepository;
        }

        /// <inheritdoc />
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
