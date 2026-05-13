using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Features.MovieSwipe;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using System.Threading.Tasks;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/swipe")]
public sealed class SwipeEndpointsController : ControllerBase
{
    private readonly ISwipeService _swipeService;

    public SwipeEndpointsController(ISwipeService swipeService)
    {
        _swipeService = swipeService;
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePreferenceScoreAsync([FromBody] UpdatePreferenceScoreRequestBody request)
    {
        await _swipeService.UpdatePreferenceScoreAsync(request.UserId, request.MovieId, request.IsLiked);
        return Ok();
    }
}
