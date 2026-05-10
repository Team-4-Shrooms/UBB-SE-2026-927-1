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
    }
}

