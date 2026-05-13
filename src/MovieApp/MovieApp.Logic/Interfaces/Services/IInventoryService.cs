using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IInventoryService
    {
        Task RemoveOwnedMovieAsync(int userId, int movieId);
        Task RemoveOwnedTicketAsync(int userId, int eventId);
        Task<List<Movie>> GetOwnedMoviesAsync(int userId);
        Task<List<OwnedTicket>> GetOwnedTicketsAsync(int userId);
        Task<List<OwnedMovie>> GetMovieOwnershipsAsync(int userId, int movieId);
        Task RemoveMovieOwnershipsAsync(IEnumerable<int> ownershipIds);
        Task<List<OwnedTicket>> GetTicketOwnershipsAsync(int userId, int eventId);
        Task RemoveTicketOwnershipsAsync(IEnumerable<int> ownershipIds);
        Task AddOwnedMovieAsync(int userId, int movieId);
    }
}

