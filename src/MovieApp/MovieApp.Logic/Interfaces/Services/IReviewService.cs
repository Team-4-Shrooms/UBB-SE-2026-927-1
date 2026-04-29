using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.DataLayer.Interfaces.Services
{
    public interface IReviewService
    {
        Task<int[]> GetStarRatingBucketsAsync(int movieId);
        Task PostReviewAsync(int movieId, int userId, int rating, string? comment);
    }
}

