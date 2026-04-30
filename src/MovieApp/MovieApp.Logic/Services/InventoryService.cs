using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Services
{
    public class InventoryService : IInventoryService
    {
        private const string RemoveOwnedMovieTransactionType = "RemoveOwnedMovie";
        private const string RemoveOwnedTicketTransactionType = "RemoveOwnedTicket";
        private const string CompletedTransactionStatus = "Completed";

        private readonly IInventoryRepository _inventoryRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMovieRepository _movieRepo;
        private readonly IEventRepository _eventRepo;

        public InventoryService(IInventoryRepository inventoryRepo, IUserRepository userRepo, IMovieRepository movieRepo, IEventRepository eventRepo)
        {
            _inventoryRepo = inventoryRepo;
            _userRepo = userRepo;
            _movieRepo = movieRepo;
            _eventRepo = eventRepo;
        }

        public async Task RemoveOwnedMovieAsync(int userId, int movieId)
        {
            User user = await _userRepo.GetUserByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            Movie movie = await _movieRepo.GetMovieByIdAsync(movieId)
                ?? throw new KeyNotFoundException($"Movie {movieId} not found.");

            List<OwnedMovie> ownerships = await _inventoryRepo.GetMovieOwnershipsAsync(userId, movieId);

            await _inventoryRepo.RemoveMovieOwnershipsAsync(ownerships);

            await _inventoryRepo.AddTransactionAsync(new Transaction
            {
                Buyer = user,
                Movie = movie,
                Amount = 0m,
                Type = RemoveOwnedMovieTransactionType,
                Status = CompletedTransactionStatus,
                Timestamp = DateTime.UtcNow
            });

            await _inventoryRepo.SaveChangesAsync();
        }

        public async Task RemoveOwnedTicketAsync(int userId, int eventId)
        {
            User user = await _userRepo.GetUserByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            MovieEvent movieEvent = await _eventRepo.GetEventByIdAsync(eventId)
                ?? throw new KeyNotFoundException($"Event {eventId} not found.");

            List<OwnedTicket> ownerships = await _inventoryRepo.GetTicketOwnershipsAsync(userId, eventId);

            await _inventoryRepo.RemoveTicketOwnershipsAsync(ownerships);

            await _inventoryRepo.AddTransactionAsync(new Transaction
            {
                Buyer = user,
                Event = movieEvent,
                Amount = 0m,
                Type = RemoveOwnedTicketTransactionType,
                Status = CompletedTransactionStatus,
                Timestamp = DateTime.UtcNow
            });

            await _inventoryRepo.SaveChangesAsync();
        }
    }
}

