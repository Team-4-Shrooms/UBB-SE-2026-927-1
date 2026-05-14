using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Features.MovieTournament;
using MovieApp.Logic.Interfaces;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.Models;

namespace MovieApp.Web.Controllers
{
    public class TournamentController : Controller
    {
        private readonly ITournamentLogicService _tournamentLogicService;
        private readonly IMovieTournamentService _movieTournamentService;
        private readonly ICurrentUserService _currentUserService;

        public TournamentController(
            ITournamentLogicService tournamentLogicService,
            IMovieTournamentService movieTournamentService,
            ICurrentUserService currentUserService)
        {
            _tournamentLogicService = tournamentLogicService;
            _movieTournamentService = movieTournamentService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Setup));
        }

        [HttpGet]
        public async Task<IActionResult> Setup()
        {
            int available = await _movieTournamentService.GetTournamentPoolSizeAsync(_currentUserService.UserId);
            return View(new TournamentSetupViewModel { AvailableCount = available });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int poolSize)
        {
            try
            {
                await _tournamentLogicService.StartTournamentAsync(_currentUserService.UserId, poolSize);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = $"Not enough rated movies to start a {poolSize}-movie tournament.";
                return RedirectToAction(nameof(Setup));
            }
            return RedirectToAction(nameof(Match));
        }

        [HttpGet]
        public async Task<IActionResult> Match()
        {
            var matchPair = await _tournamentLogicService.GetCurrentMatchAsync(_currentUserService.UserId);

            if (matchPair == null)
            {
                bool isComplete = await _tournamentLogicService.IsTournamentCompleteAsync(_currentUserService.UserId);
                if (isComplete)
                {
                    return RedirectToAction(nameof(Winner));
                }
                return RedirectToAction(nameof(Setup));
            }

            var viewModel = new TournamentMatchViewModel
            {
                Left = matchPair.FirstMovie,
                Right = matchPair.SecondMovie,
                CurrentRound = 1, 
                TotalRounds = 1
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PickWinner(int winnerMovieId)
        {
            await _tournamentLogicService.AdvanceWinnerAsync(_currentUserService.UserId, winnerMovieId);

            bool isComplete = await _tournamentLogicService.IsTournamentCompleteAsync(_currentUserService.UserId);
            if (isComplete)
            {
                return RedirectToAction(nameof(Winner));
            }

            return RedirectToAction(nameof(Match));
        }

        [HttpGet]
        public async Task<IActionResult> Winner()
        {
            try
            {
                var winner = await _tournamentLogicService.GetFinalWinnerAsync(_currentUserService.UserId);
                var viewModel = new TournamentWinnerViewModel { Winner = winner };
                return View(viewModel);
            }
            catch (InvalidOperationException)
            {
                return RedirectToAction(nameof(Setup));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset()
        {
            await _tournamentLogicService.ResetTournamentAsync(_currentUserService.UserId);
            return RedirectToAction(nameof(Setup));
        }
    }
}
