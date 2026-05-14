using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            return await _userRepository.GetBalanceAsync(userId);
        }

        public async Task UpdateBalanceAsync(int userId, decimal newBalance)
        {
            await _userRepository.UpdateBalanceAsync(userId, newBalance);
        }
    }
}
