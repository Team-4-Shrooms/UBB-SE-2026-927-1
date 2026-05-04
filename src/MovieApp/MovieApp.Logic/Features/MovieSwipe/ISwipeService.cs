namespace MovieApp.Logic.Features.MovieSwipe
{
    /// <summary>
    /// Defines the business logic for processing user swipe actions.
    /// </summary>
    public interface ISwipeService
    {
        Task UpdatePreferenceScoreAsync(int userId, int movieId, bool isLiked);
    }
}
