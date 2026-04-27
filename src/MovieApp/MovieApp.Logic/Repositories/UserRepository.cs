using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public decimal GetBalance(int userId)
        {
            if (userId <= 0)
            {
                return 0m;
            }

            return _context.Users
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

            User? user = _context.Users.FirstOrDefault(candidate => candidate.Id == userId);
            if (user is null)
            {
                return;
            }

            user.Balance = newBalance;
            _context.SaveChanges();
        }
    }
}
