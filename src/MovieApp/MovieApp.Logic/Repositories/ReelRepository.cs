using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MovieApp.Logic.Repositories
{
    /// <summary>
    /// EF Core data access for the Reel table.
    /// </summary>
    public class ReelRepository : IReelRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReelRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public ReelRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IList<Reel>> GetUserReelsAsync(int userId)
        {
            return await _context.Reels
                .Include(reel => reel.Movie)
                .Include(reel => reel.CreatorUser)
                .Where(reel => reel.CreatorUser.Id == userId)
                .OrderByDescending(reel => reel.CreatedAt)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Reel?> GetReelByIdAsync(int reelId)
        {
            return await _context.Reels
                .Include(reel => reel.Movie)
                .Include(reel => reel.CreatorUser)
                .FirstOrDefaultAsync(reel => reel.Id == reelId);
        }

        /// <inheritdoc />
        public async Task<int> UpdateReelEditsAsync(int reelId, string cropDataJson, int? backgroundMusicId, string videoUrl)
        {
            Reel? reel = await _context.Reels.FindAsync(reelId);

            if (reel is null)
            {
                return 0;
            }

            reel.CropDataJson = cropDataJson;
            reel.BackgroundMusicId = backgroundMusicId;
            reel.VideoUrl = string.IsNullOrEmpty(videoUrl) ? reel.VideoUrl : videoUrl;
            reel.LastEditedAt = DateTime.UtcNow;

            return await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteReelAsync(int reelId)
        {
            Reel? reel = await _context.Reels.FindAsync(reelId);

            if (reel is null)
            {
                return;
            }

            List<UserReelInteraction> interactions = await _context.UserReelInteractions
                .Where(interaction => interaction.Reel.Id == reelId)
                .ToListAsync();

            _context.UserReelInteractions.RemoveRange(interactions);
            _context.Reels.Remove(reel);

            await _context.SaveChangesAsync();
        }
    }
}
