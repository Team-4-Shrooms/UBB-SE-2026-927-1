using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Features.PersonalityMatch;
using MovieApp.Logic.Interfaces;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.Models;

namespace MovieApp.Web.Controllers
{
    public class PersonalityMatchController : Controller
    {
        private const int TotalQuestions = 10;
        private const int TopPreferencesCount = TotalQuestions;
        private const int TopMatchesCount = 5;

        private readonly IPersonalityMatchService _personalityMatchService;
        private readonly IPersonalityMatchingService _personalityMatchingService;
        private readonly ICurrentUserService _currentUserService;

        public PersonalityMatchController(
            IPersonalityMatchService personalityMatchService,
            IPersonalityMatchingService personalityMatchingService,
            ICurrentUserService currentUserService)
        {
            _personalityMatchService = personalityMatchService;
            _personalityMatchingService = personalityMatchingService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = _currentUserService.UserId;
            string sessionKey = $"personality:answers:{userId}";

            var answersJson = HttpContext.Session.GetString(sessionKey);
            var answers = answersJson != null
                ? JsonSerializer.Deserialize<List<string>>(answersJson) ?? new()
                : new List<string>();

            int questionIndex = answers.Count;

            if (questionIndex >= TotalQuestions)
            {
                return RedirectToAction(nameof(Results));
            }

            var preferences = await _personalityMatchingService.GetTopMoviePreferencesAsync(userId, TopPreferencesCount);

            if (preferences == null || preferences.Count == 0)
            {
                return RedirectToAction(nameof(Results));
            }

            // Each preference's Title becomes a question option
            var allTitles = preferences.Select(p => p.Title).Distinct().ToList();

            int optionCount = Math.Min(4, allTitles.Count);
            int startIndex = (questionIndex * optionCount) % Math.Max(allTitles.Count, 1);
            var options = allTitles.Skip(startIndex).Take(optionCount).ToList();

            if (options.Count < 2 && allTitles.Count >= 2)
            {
                options = allTitles.Take(4).ToList();
            }

            var viewModel = new PersonalityMatchIndexViewModel
            {
                QuestionIndex = questionIndex,
                TotalQuestions = TotalQuestions,
                QuestionText = "Which of these movies would you most like to watch?",
                Options = options,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(int questionIndex, string answer)
        {
            int userId = _currentUserService.UserId;
            string sessionKey = $"personality:answers:{userId}";

            var answersJson = HttpContext.Session.GetString(sessionKey);
            var answers = answersJson != null
                ? JsonSerializer.Deserialize<List<string>>(answersJson) ?? new()
                : new List<string>();

            answers.Add(answer);
            HttpContext.Session.SetString(sessionKey, JsonSerializer.Serialize(answers));

            if (answers.Count >= TotalQuestions)
            {
                var matches = await _personalityMatchingService.GetTopMatchesAsync(userId, TopMatchesCount);
                TempData["personality:results"] = JsonSerializer.Serialize(matches);
                HttpContext.Session.Remove(sessionKey);
                return RedirectToAction(nameof(Results));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Results()
        {
            int userId = _currentUserService.UserId;

            List<MatchResult> matches;

            var resultsJson = TempData["personality:results"] as string;
            if (!string.IsNullOrEmpty(resultsJson))
            {
                matches = JsonSerializer.Deserialize<List<MatchResult>>(resultsJson) ?? new();
            }
            else
            {
                matches = await _personalityMatchingService.GetTopMatchesAsync(userId, TopMatchesCount);
            }

            var viewModel = new PersonalityMatchResultsViewModel
            {
                Matches = matches.Select(m => new PersonalityMatchResultViewModel
                {
                    UserId = m.MatchedUserId,
                    Username = m.MatchedUsername,
                    MatchScore = m.MatchScore,
                    CommonPreferences = new List<string>(), // MatchResult has no common preferences field
                }).ToList(),
            };

            return View(viewModel);
        }
    }
}
