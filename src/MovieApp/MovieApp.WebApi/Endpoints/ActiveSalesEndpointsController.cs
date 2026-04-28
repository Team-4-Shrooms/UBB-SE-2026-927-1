using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.DTO.WebAPI;
using MovieApp.DataLayer.Repositories;

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
    public IActionResult GetCurrentSales()
    {
        var currentSales = _repository.GetCurrentSales().Select(sale => sale.ToDto());

        return Ok(currentSales);
    }

    [HttpGet("best-discounts")]
    public IActionResult GetBestDiscountPercentByMovieId()
    {
        return Ok(_repository.GetBestDiscountPercentByMovieId());
    }
}
