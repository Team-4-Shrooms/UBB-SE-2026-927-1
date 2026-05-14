using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IReelService
    {
        Task<IList<Reel>> GetUserReelsAsync(int userId);
        Task<Reel?> GetReelByIdAsync(int reelId);
        Task<int> UpdateReelEditsAsync(int reelId, string cropDataJson, int? backgroundMusicId, string videoUrl);
        Task DeleteReelAsync(int reelId);
    }
}
