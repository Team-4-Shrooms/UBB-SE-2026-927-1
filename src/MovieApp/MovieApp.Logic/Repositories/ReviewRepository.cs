using Microsoft.EntityFrameworkCore;
using MovieApp.Logic.Data;
using MovieApp.Logic.Interfaces.Repositories;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Repositories
{
    public sealed class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MovieReview>> GetReviewsForMovieAsync(int movieId) =>
        await _context.MovieReviews.Include(r => r.User).Where(r => r.Movie.Id == movieId)
            .OrderByDescending(r => r.CreatedAt).ToListAsync();

        public async Task<List<decimal>> GetRawRatingsForMovieAsync(int movieId) =>
            await _context.MovieReviews.Where(r => r.Movie.Id == movieId).Select(r => r.StarRating).ToListAsync();

        public async Task<Dictionary<int, int>> GetReviewCountsAsync(IEnumerable<int> movieIds) =>
            await _context.MovieReviews.Where(r => movieIds.Contains(r.Movie.Id))
                .GroupBy(r => r.Movie.Id).Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

        public async Task AddReviewAsync(MovieReview review) => await _context.MovieReviews.AddAsync(review);
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
