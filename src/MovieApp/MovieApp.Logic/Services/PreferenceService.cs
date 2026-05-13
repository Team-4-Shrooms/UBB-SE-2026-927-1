using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public class PreferenceService : IPreferenceService
    {
        private readonly IPreferenceRepository _preferenceRepository;

        public PreferenceService(IPreferenceRepository preferenceRepository)
        {
            _preferenceRepository = preferenceRepository;
        }

        public async Task<bool> PreferenceExistsAsync(int userId, int movieId)
        {
            return await _preferenceRepository.PreferenceExistsAsync(userId, movieId);
        }

        public async Task InsertPreferenceAsync(int userId, int movieId, decimal score)
        {
            await _preferenceRepository.InsertPreferenceAsync(userId, movieId, score);
        }

        public async Task UpdatePreferenceAsync(int userId, int movieId, decimal boost)
        {
            await _preferenceRepository.UpdatePreferenceAsync(userId, movieId, boost);
        }
    }
}
