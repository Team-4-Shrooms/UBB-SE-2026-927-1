using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Defines methods for accessing and managing a user's owned movies, tickets, and equipment in the inventory
    /// system.
    /// </summary>
    public interface IInventoryRepository
    {
        Task<List<Movie>> GetOwnedMoviesAsync(int userId);
        Task<List<OwnedMovie>> GetMovieOwnershipsAsync(int userId, int movieId);
        Task RemoveMovieOwnershipsAsync(IEnumerable<OwnedMovie> ownerships);
        Task<List<OwnedTicket>> GetTicketOwnershipsAsync(int userId, int eventId);
        Task RemoveTicketOwnershipsAsync(IEnumerable<OwnedTicket> ownerships);
        Task AddTransactionAsync(Transaction transaction);
        Task<int> SaveChangesAsync();
    }
}

