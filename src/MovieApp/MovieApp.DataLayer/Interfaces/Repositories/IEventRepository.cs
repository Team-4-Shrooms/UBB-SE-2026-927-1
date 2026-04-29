using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing movie events.
    /// </summary>
    public interface IEventRepository
    {
        Task<List<MovieEvent>> GetEventsByMovieIdAsync(int movieId);
        Task<List<MovieEvent>> GetAllEventsAsync();
        Task<MovieEvent?> GetEventByIdAsync(int eventId);
        Task<bool> UserHasTicketAsync(int userId, int eventId);
        Task AddOwnedTicketAsync(OwnedTicket ticket);
        Task AddTransactionAsync(Transaction transaction);
        Task<int> SaveChangesAsync();
    }
}

