namespace MovieApp.Features.MovieSwipe.Services
{
    /// <summary>
    /// Defines the business logic for processing user swipe actions.
    /// </summary>
    public interface ISwipeService
    {
        /// <summary> Processes a swipe action by calculating the score delta and updating the repository. </summary>
        /// <param name="userId">The user performing the swipe.</param>
        /// <param name="movieId">The movie being swiped on.</param>
        /// <param name="isLiked">True if the user swiped right (like), false if they swiped left (skip).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdatePreferenceScoreAsync(int userId, int movieId, bool isLiked);
    }
}