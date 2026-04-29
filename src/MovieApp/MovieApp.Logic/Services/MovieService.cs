using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.DataLayer.Models;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepo;
    private readonly IUserRepository _userRepo;

    public MovieService(IMovieRepository movieRepo, IUserRepository userRepo)
    {
        _movieRepo = movieRepo;
        _userRepo = userRepo;
    }
    public async Task PurchaseMovieAsync(int userId, int movieId, decimal price)
    {
        var user = await _userRepo.GetUserByIdAsync(userId)
                   ?? throw new KeyNotFoundException("User not found.");

        var movie = await _movieRepo.GetMovieByIdAsync(movieId)
                    ?? throw new KeyNotFoundException("Movie not found.");

        if (await _movieRepo.UserOwnsMovieAsync(userId, movieId))
            throw new InvalidOperationException("Movie already owned.");

        if (user.Balance < price)
            throw new InvalidOperationException("Insufficient balance.");

        user.Balance -= price;

        await _movieRepo.AddOwnedMovieAsync(new OwnedMovie
        {
            User = user,
            Movie = movie,
            PurchaseDate = DateTime.UtcNow
        });

        await _movieRepo.AddTransactionAsync(new Transaction
        {
            Buyer = user,
            Movie = movie,
            Amount = -price,
            Type = "MoviePurchase",
            Status = "Completed",
            Timestamp = DateTime.UtcNow
        });

        await _movieRepo.SaveChangesAsync();
    }

    public async Task<List<Movie>> SearchMoviesAsync(string? query)
    {
        return await _movieRepo.SearchMoviesAsync(query ?? string.Empty, 10);
    }

}

