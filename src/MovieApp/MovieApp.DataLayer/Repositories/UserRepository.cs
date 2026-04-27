using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly MovieApp.Logic.Data.IMovieAppDbContext _context;
        private DbSet<User> Users => _context.Users;

        public UserRepository(MovieApp.Logic.Data.IMovieAppDbContext context)
        {
            _context = context;
        }

        public decimal GetBalance(int userId)
        {
            if (userId <= 0)
            {
                return 0m;
            }

            return Users
                .Where(user => user.Id == userId)
                .Select(user => (decimal?)user.Balance)
                .FirstOrDefault() ?? 0m;
        }

        public void UpdateBalance(int userId, decimal newBalance)
        {
            if (userId <= 0)
            {
                return;
            }

            User? user = Users.FirstOrDefault(candidate => candidate.Id == userId);
            if (user is null)
            {
                return;
            }

            user.Balance = newBalance;
            _context.SaveChanges();
        }
    }
}
