using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.MovieTournament;

namespace MovieApp.WebApi.Endpoints
{
    [Authorize]
    [ApiController]
    [Route("api/tournament-game")]
    public class TournamentGameController : ControllerBase
    {
        private readonly ITournamentLogicService _tournamentLogicService;

        public TournamentGameController(ITournamentLogicService tournamentLogicService)
        {
            _tournamentLogicService = tournamentLogicService;
        }

        [HttpPost("{userId}/start")]
        public async Task<IActionResult> StartTournamentAsync(int userId, [FromQuery] int poolSize)
        {
            await _tournamentLogicService.StartTournamentAsync(userId, poolSize);
            return Ok();
        }

        [HttpPost("{userId}/advance")]
        public async Task<IActionResult> AdvanceWinnerAsync(int userId, [FromBody] int winnerId)
        {
            await _tournamentLogicService.AdvanceWinnerAsync(userId, winnerId);
            return Ok();
        }

        [HttpGet("{userId}/current-match")]
        public async Task<ActionResult<MatchPair>> GetCurrentMatchAsync(int userId)
        {
            var match = await _tournamentLogicService.GetCurrentMatchAsync(userId);
            if (match == null)
            {
                return NoContent(); // No active match found
            }
            return Ok(match);
        }

        [HttpGet("{userId}/is-complete")]
        public async Task<ActionResult<bool>> IsTournamentCompleteAsync(int userId)
        {
            var isComplete = await _tournamentLogicService.IsTournamentCompleteAsync(userId);
            return Ok(isComplete);
        }

        [HttpGet("{userId}/winner")]
        public async Task<ActionResult<Movie>> GetFinalWinnerAsync(int userId)
        {
            try
            {
                var winner = await _tournamentLogicService.GetFinalWinnerAsync(userId);
                return Ok(winner);
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Tournament is not complete or no winner exists.");
            }
        }

        [HttpPost("{userId}/reset")]
        public async Task<IActionResult> ResetTournamentAsync(int userId)
        {
            await _tournamentLogicService.ResetTournamentAsync(userId);
            return Ok();
        }
    }
}
