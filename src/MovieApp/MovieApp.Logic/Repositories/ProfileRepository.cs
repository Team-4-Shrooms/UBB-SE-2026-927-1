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
    /// EF Core data access for the UserProfile table.
    /// </summary>
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetProfileAsync(int userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(p => p.User.Id == userId);
        }

        public async Task<List<UserReelInteraction>> GetInteractionsAsync(int userId)
        {
            return await _context.UserReelInteractions.Where(i => i.User.Id == userId).ToListAsync();
        }

        public async Task AddProfileAsync(UserProfile profile)
        {
            await _context.UserProfiles.AddAsync(profile);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
