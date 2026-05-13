using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Features.MovieSwipe;
using MovieApp.Logic.Interfaces;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.Models;

namespace MovieApp.Web.Controllers
{
    public class SwipeController : Controller
    {
        private const int FeedBatchSize = 20;
        private const string LikedKey = "swipe:liked";
        private const string DislikedKey = "swipe:disliked";
        private const string FeedKey = "swipe:feed";

        private readonly IMovieCardFeedService _movieCardFeedService;
        private readonly ISwipeService _swipeService;
        private readonly ICurrentUserService _currentUserService;

        public SwipeController(
            IMovieCardFeedService movieCardFeedService,
            ISwipeService swipeService,
            ICurrentUserService currentUserService)
        {
            _movieCardFeedService = movieCardFeedService;
            _swipeService = swipeService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = _currentUserService.UserId;

            // Try to restore feed from session to avoid re-fetching on every card
            var feedJson = HttpContext.Session.GetString(FeedKey);
            var feed = feedJson != null
                ? JsonSerializer.Deserialize<List<MovieApp.DataLayer.Models.Movie>>(feedJson) ?? new()
                : await _movieCardFeedService.FetchMovieFeedAsync(userId, FeedBatchSize);

            if (feed.Count == 0)
            {
                return RedirectToAction(nameof(Summary));
            }

            // Persist remaining feed back to session
            HttpContext.Session.SetString(FeedKey, JsonSerializer.Serialize(feed));

            int liked = HttpContext.Session.GetInt32(LikedKey) ?? 0;
            int disliked = HttpContext.Session.GetInt32(DislikedKey) ?? 0;

            var viewModel = new SwipeCardViewModel
            {
                Movie = feed[0],
                LikedCount = liked,
                DislikedCount = disliked,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Swipe(int movieId, bool liked)
        {
            int userId = _currentUserService.UserId;

            await _swipeService.UpdatePreferenceScoreAsync(userId, movieId, liked);

            // Update session counters
            if (liked)
            {
                HttpContext.Session.SetInt32(LikedKey, (HttpContext.Session.GetInt32(LikedKey) ?? 0) + 1);
            }
            else
            {
                HttpContext.Session.SetInt32(DislikedKey, (HttpContext.Session.GetInt32(DislikedKey) ?? 0) + 1);
            }

            // Remove the swiped movie from the cached feed
            var feedJson = HttpContext.Session.GetString(FeedKey);
            if (feedJson != null)
            {
                var feed = JsonSerializer.Deserialize<List<MovieApp.DataLayer.Models.Movie>>(feedJson) ?? new();
                feed.RemoveAll(m => m.Id == movieId);
                HttpContext.Session.SetString(FeedKey, JsonSerializer.Serialize(feed));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Summary()
        {
            var viewModel = new SwipeSummaryViewModel
            {
                LikedCount = HttpContext.Session.GetInt32(LikedKey) ?? 0,
                DislikedCount = HttpContext.Session.GetInt32(DislikedKey) ?? 0,
            };

            // Clear swipe session state
            HttpContext.Session.Remove(LikedKey);
            HttpContext.Session.Remove(DislikedKey);
            HttpContext.Session.Remove(FeedKey);

            return View(viewModel);
        }
    }
}
