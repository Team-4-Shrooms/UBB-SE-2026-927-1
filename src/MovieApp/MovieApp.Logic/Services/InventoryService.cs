using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<Movie>> GetOwnedMoviesAsync(int userId)
        {
            return await _inventoryRepo.GetOwnedMoviesAsync(userId);
        }

        public async Task<List<OwnedTicket>> GetOwnedTicketsAsync(int userId)
        {
            return await _inventoryRepo.GetAllTicketsForUserAsync(userId);
        }

        public async Task<List<OwnedMovie>> GetMovieOwnershipsAsync(int userId, int movieId)
        {
            return await _inventoryRepo.GetMovieOwnershipsAsync(userId, movieId);
        }

        public async Task RemoveMovieOwnershipsAsync(IEnumerable<int> ownershipIds)
        {
            var ownerships = ownershipIds.Select(id => new OwnedMovie { Id = id });
            await _inventoryRepo.RemoveMovieOwnershipsAsync(ownerships);
            await _inventoryRepo.SaveChangesAsync();
        }

        public async Task<List<OwnedTicket>> GetTicketOwnershipsAsync(int userId, int eventId)
        {
            return await _inventoryRepo.GetTicketOwnershipsAsync(userId, eventId);
        }

        public async Task RemoveTicketOwnershipsAsync(IEnumerable<int> ownershipIds)
        {
            var ownerships = ownershipIds.Select(id => new OwnedTicket { Id = id });
            await _inventoryRepo.RemoveTicketOwnershipsAsync(ownerships);
            await _inventoryRepo.SaveChangesAsync();
        }

        public async Task AddOwnedMovieAsync(int userId, int movieId)
        {
            User user = await _userRepo.GetUserByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");
            Movie movie = await _movieRepo.GetMovieByIdAsync(movieId)
                ?? throw new KeyNotFoundException($"Movie {movieId} not found.");
            await _movieRepo.AddOwnedMovieAsync(new OwnedMovie { User = user, Movie = movie });
            await _movieRepo.SaveChangesAsync();
        }
    }
}

