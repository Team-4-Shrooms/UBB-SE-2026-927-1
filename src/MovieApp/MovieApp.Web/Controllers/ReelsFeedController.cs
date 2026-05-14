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

            var rawReels = await _recommendationService.GetRecommendedReelsAsync(userId, 20);
            var allLikeCounts = await _recommendationService.GetAllLikeCountsAsync();
            var userInteractions = await _interactionService.GetInteractionsForUserAsync(userId);

            var likedReelIds = userInteractions
                .Where(i => i.IsLiked)
                .Select(i => i.ReelId)
                .ToHashSet();

            var reels = rawReels.Select(r => new ReelDisplayItem
            {
                Id = r.Id,
                Title = r.Title,
                VideoUrl = r.VideoUrl,
                ThumbnailUrl = r.ThumbnailUrl,
                Caption = r.Caption,
                LikeCount = allLikeCounts.TryGetValue(r.Id, out var count) ? count : 0,
                IsLikedByMe = likedReelIds.Contains(r.Id),
            }).ToList();

            var viewModel = new ReelsFeedViewModel
            {
                CurrentPage = page,
                Reels = reels
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> Like(int reelId)
        {
            var userId = _currentUserService.UserId;
            await _interactionService.ToggleLikeAsync(userId, reelId);

            var newCount = await _interactionService.GetLikeCountAsync(reelId);
            var interaction = await _interactionService.GetInteractionAsync(userId, reelId);

            return Json(new { likeCount = newCount, isLiked = interaction?.IsLiked ?? false });
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
