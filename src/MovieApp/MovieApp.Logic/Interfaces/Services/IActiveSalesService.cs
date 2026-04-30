using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IActiveSalesService
    {
        /// <summary>
        /// Retrieves a dictionary mapping movie IDs to their best available discount percentages.
        /// </summary>
        /// <returns>Dictionary where the key is the movie ID and the value is the best discount percentage available for that movie.</returns>
        Task<Dictionary<int, decimal>> GetBestDiscountPercentByMovieIdAsync();
    }
}

