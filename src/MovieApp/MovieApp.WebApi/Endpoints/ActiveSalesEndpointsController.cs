using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Repositories;
using MovieApp.WebApi.Mappings;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/active-sales")]
public sealed class ActiveSalesEndpointsController : ControllerBase
{
    private readonly ActiveSalesRepository _repository;

    public ActiveSalesEndpointsController(ActiveSalesRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentSales()
    {
        var currentSales = (await _repository.GetCurrentSalesAsync())
            .Select(sale => sale.ToDto());
        return Ok(currentSales);
    }
}
