namespace MovieApp.Logic.Interfaces.Repositories
{
    public interface IUserRepository
    {
        decimal GetBalance(int userId);

        void UpdateBalance(int userId, decimal newBalance);
    }
}
