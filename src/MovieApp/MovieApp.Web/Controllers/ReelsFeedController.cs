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
        private readonly string _webApiBaseUrl;

        public ReelsFeedController(
            IRecommendationService recommendationService,
            IReelInteractionService interactionService,
            ICurrentUserService currentUserService,
            IConfiguration configuration)
        {
            _recommendationService = recommendationService;
            _interactionService = interactionService;
            _currentUserService = currentUserService;
            _webApiBaseUrl = configuration["WebApi:BaseUrl"]!.TrimEnd('/');
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = _currentUserService.UserId;

            var rawReels = await _recommendationService.GetRecommendedReelsAsync(userId, 20);
            var allLikeCounts = await _recommendationService.GetAllLikeCountsAsync();
            var userInteractions = await _interactionService.GetInteractionsForUserAsync(userId);

            var likedReelIds = userInteractions
                .Where(interaction => interaction.IsLiked)
                .Select(interaction => interaction.ReelId)
                .ToHashSet();

            var reels = rawReels.Select(reel => new ReelDisplayItem
            {
                Id = reel.Id,
                Title = reel.Title,
                VideoUrl = reel.VideoUrl.StartsWith("/") ? _webApiBaseUrl + reel.VideoUrl : reel.VideoUrl,
                ThumbnailUrl = reel.ThumbnailUrl,
                Caption = reel.Caption,
                LikeCount = allLikeCounts.TryGetValue(reel.Id, out var count) ? count : 0,
                IsLikedByMe = likedReelIds.Contains(reel.Id),
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
