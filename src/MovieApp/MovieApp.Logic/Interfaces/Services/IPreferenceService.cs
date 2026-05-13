using System.Threading.Tasks;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IPreferenceService
    {
        Task<bool> PreferenceExistsAsync(int userId, int movieId);
        Task InsertPreferenceAsync(int userId, int movieId, decimal score);
        Task UpdatePreferenceAsync(int userId, int movieId, decimal boost);
    }
}
