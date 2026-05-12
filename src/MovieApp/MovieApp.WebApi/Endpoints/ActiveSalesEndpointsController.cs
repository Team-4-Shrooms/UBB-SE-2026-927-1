using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/active-sales")]
public sealed class ActiveSalesEndpointsController : ControllerBase
{
    private readonly IActiveSalesService _activeSalesService;

    public ActiveSalesEndpointsController(IActiveSalesService activeSalesService)
    {
        _activeSalesService = activeSalesService;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentSales()
    {
        var currentSales = await _activeSalesService.GetBestDiscountPercentByMovieIdAsync();
        return Ok(currentSales);
    }
}
