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
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;

        public MovieService(IMovieRepository movieRepository, IUserRepository userRepository)
        {
            _movieRepository = movieRepository;
            _userRepository = userRepository;
        }
        public async Task PurchaseMovieAsync(int userId, int movieId, decimal price)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                       ?? throw new KeyNotFoundException("User not found.");

            var movie = await _movieRepository.GetMovieByIdAsync(movieId)
                        ?? throw new KeyNotFoundException("Movie not found.");

            if (await _movieRepository.UserOwnsMovieAsync(userId, movieId))
            {
                throw new InvalidOperationException("Movie already owned.");
            }

            if (user.Balance < price)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            user.Balance -= price;

            await _movieRepository.AddOwnedMovieAsync(new OwnedMovie
            {
                User = user,
                Movie = movie,
                PurchaseDate = DateTime.UtcNow
            });

            await _movieRepository.AddTransactionAsync(new Transaction
            {
                Buyer = user,
                Movie = movie,
                Amount = -price,
                Type = "MoviePurchase",
                Status = "Completed",
                Timestamp = DateTime.UtcNow
            });

            await _movieRepository.SaveChangesAsync();
        }

        public async Task<List<Movie>> SearchMoviesAsync(string? query)
        {
            return await _movieRepository.SearchMoviesAsync(query ?? string.Empty, 10);
        }

    }
}

