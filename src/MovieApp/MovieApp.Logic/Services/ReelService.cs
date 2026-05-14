using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public class ReelService : IReelService
    {
        private readonly IReelRepository _reelRepository;

        public ReelService(IReelRepository reelRepository)
        {
            _reelRepository = reelRepository;
        }

        public async Task<IList<Reel>> GetUserReelsAsync(int userId)
        {
            return await _reelRepository.GetUserReelsAsync(userId);
        }

        public async Task<Reel?> GetReelByIdAsync(int reelId)
        {
            return await _reelRepository.GetReelByIdAsync(reelId);
        }

        public async Task<int> UpdateReelEditsAsync(int reelId, string cropDataJson, int? backgroundMusicId, string videoUrl)
        {
            return await _reelRepository.UpdateReelEditsAsync(reelId, cropDataJson, backgroundMusicId, videoUrl);
        }

        public async Task DeleteReelAsync(int reelId)
        {
            await _reelRepository.DeleteReelAsync(reelId);
        }
    }
}
