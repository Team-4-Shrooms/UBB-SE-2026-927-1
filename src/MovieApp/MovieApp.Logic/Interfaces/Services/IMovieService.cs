using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IMovieService
    {
        Task PurchaseMovieAsync(int userId, int movieId, decimal price);
        Task<List<Movie>> SearchMoviesAsync(string? partialName);
    }
}
