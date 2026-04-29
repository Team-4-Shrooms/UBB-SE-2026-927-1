using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Interfaces.Services;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepo;
        private readonly IUserRepository _userRepo;

        public EventService(IEventRepository eventRepo, IUserRepository userRepo)
        {
            _eventRepo = eventRepo;
            _userRepo = userRepo;
        }

        public async Task<List<MovieEvent>> GetAvailableEventsAsync() =>
            await _eventRepo.GetAllEventsAsync();

        public async Task PurchaseTicketAsync(int userId, int eventId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId)
                       ?? throw new InvalidOperationException("User not found.");

            var movieEvent = await _eventRepo.GetEventByIdAsync(eventId)
                             ?? throw new InvalidOperationException("Event not found.");

            if (await _eventRepo.UserHasTicketAsync(userId, eventId))
            {
                throw new InvalidOperationException("You already own a ticket for this event.");
            }

            decimal price = movieEvent.TicketPrice;
            if (user.Balance < price)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            user.Balance -= price;

            await _eventRepo.AddOwnedTicketAsync(new OwnedTicket
            {
                User = user,
                Event = movieEvent,
                PurchaseDate = DateTime.UtcNow
            });

            await _eventRepo.AddTransactionAsync(new Transaction
            {
                Buyer = user,
                Event = movieEvent,
                Amount = -price,
                Type = "EventTicket",
                Status = "Completed",
                Timestamp = DateTime.UtcNow
            });

            await _eventRepo.SaveChangesAsync();
        }
    }
}

