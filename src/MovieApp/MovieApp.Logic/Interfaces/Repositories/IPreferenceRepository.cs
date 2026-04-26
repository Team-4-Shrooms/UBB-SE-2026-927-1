namespace MovieApp.Logic.Interfaces.Repositories
{
    /// <summary>
    /// Repository for managing user movie preferences in the reels feed context.
    /// Exposes simple data access methods; preference business logic lives in services.
    /// </summary>
    public interface IPreferenceRepository
    {
        /// <summary>
        /// Checks whether a preference exists for the given user and movie.
        /// </summary>
        Task<bool> PreferenceExistsAsync(int userId, int movieId);

        /// <summary>
        /// Inserts a new preference with the provided initial score.
        /// </summary>
        Task InsertPreferenceAsync(int userId, int movieId, decimal score);

        /// <summary>
        /// Updates an existing preference by adding the provided boost to the current score.
        /// </summary>
        Task UpdatePreferenceAsync(int userId, int movieId, decimal boost);
    }
}
