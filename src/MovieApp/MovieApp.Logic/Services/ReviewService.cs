using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IMovieRepository _movieRepo;
        private readonly IUserRepository _userRepo;

        public ReviewService(IReviewRepository reviewRepository, IMovieRepository movieRepository, IUserRepository userRepository)
        {
            _movieRepo = movieRepository;
            _reviewRepo = reviewRepository;
            _userRepo = userRepository;
        }

        public async Task<int[]> GetStarRatingBucketsAsync(int movieId)
        {
            var ratings = await _reviewRepo.GetRawRatingsForMovieAsync(movieId);
            int[] counts = new int[11]; 
            foreach (var r in ratings)
            {
                int bucket = Math.Clamp((int)Math.Floor((double)r), 1, 10);
                counts[bucket]++;
            }
            return counts;
        }

        public async Task PostReviewAsync(int movieId, int userId, int rating, string? comment)
        {
            var movie = await _movieRepo.GetMovieByIdAsync(movieId) ?? throw new KeyNotFoundException("Movie not found");
            var user = await _userRepo.GetUserByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

            await _reviewRepo.AddReviewAsync(new MovieReview
            {
                Movie = movie,
                User = user,
                StarRating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            });
            await _reviewRepo.SaveChangesAsync();
        }
    }
}

