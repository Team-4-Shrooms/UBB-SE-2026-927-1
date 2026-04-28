using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    /// <summary>
    /// Repository for managing user engagement profiles in the reels feed context.
    /// </summary>
    public interface IProfileRepository
    {
        Task<UserProfile?> GetProfileAsync(int userId);
        Task<List<UserReelInteraction>> GetInteractionsAsync(int userId);
        Task AddProfileAsync(UserProfile profile);
        Task<int> SaveChangesAsync();
    }
}
