using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Features.ReelsFeed;
using MovieApp.Web.Models;

namespace MovieApp.Web.Controllers
{
    public class ReelsFeedController : Controller
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IReelInteractionService _interactionService;
        private readonly ICurrentUserService _currentUserService;

        public ReelsFeedController(
            IRecommendationService recommendationService,
            IReelInteractionService interactionService,
            ICurrentUserService currentUserService)
        {
            _recommendationService = recommendationService;
            _interactionService = interactionService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = _currentUserService.UserId;

            // 1. Get the reels from the recommendation service
            var rawReels = await _recommendationService.GetRecommendedReelsAsync(userId, 20);

            // 2. Map to ViewModel using the InteractionService for each item
            var reelTasks = rawReels.Select(async r => new ReelDisplayItem
            {
                Id = r.Id,
                Title = r.Title,
                VideoUrl = r.VideoUrl,
                ThumbnailUrl = r.ThumbnailUrl,
                Caption = r.Caption,

                // Use the service to get the actual count from the DB
                LikeCount = await _interactionService.GetLikeCountAsync(r.Id),

                // Use the service to check if THIS user liked it
                //IsLikedByCurrentUser = await _interactionService.IsLikedByUserAsync(userId, r.Id)
            });

            var viewModel = new ReelsFeedViewModel
            {
                CurrentPage = page,
                Reels = (await Task.WhenAll(reelTasks)).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> Like(int reelId)
        {
            await _interactionService.ToggleLikeAsync(_currentUserService.UserId, reelId);

            var newCount = await _interactionService.GetLikeCountAsync(reelId);

            return Json(new { likeCount = newCount });
        }

        [HttpPost]
        public async Task<JsonResult> RecordWatch(int reelId, double watchDurationSec, double watchPercentage)
        {
            await _interactionService.RecordViewAsync(
                _currentUserService.UserId,
                reelId,
                watchDurationSec,
                watchPercentage);

            return Json(new { ok = true });
        }
    }
}
