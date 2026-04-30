using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IEventService
    {
        Task<List<MovieEvent>> GetAvailableEventsAsync();
        Task PurchaseTicketAsync(int userId, int eventId);
    }
}

