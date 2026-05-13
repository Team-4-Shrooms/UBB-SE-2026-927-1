using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<decimal> GetBalanceAsync(int userId);
        Task UpdateBalanceAsync(int userId, decimal newBalance);
    }
}
