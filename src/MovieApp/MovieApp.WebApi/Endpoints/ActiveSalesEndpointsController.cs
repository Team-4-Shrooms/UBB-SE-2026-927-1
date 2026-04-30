using Microsoft.AspNetCore.Mvc;
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
        var currentSales = _repository.GetCurrentSales()
            .Select(sale => new ActiveSaleResponse
            {
                Id = sale.Id,
                DiscountPercentage = sale.DiscountPercentage,
                StartTime = sale.StartTime,
                EndTime = sale.EndTime,
                Movie = sale.Movie is null ? null : new MovieReferenceResponse
                {
                    Id = sale.Movie.Id
                }
            });
        return Ok(currentSales);
    }

    private sealed class ActiveSaleResponse
    {
        public int Id { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public MovieReferenceResponse? Movie { get; set; }
    }

    private sealed class MovieReferenceResponse
    {
        public int Id { get; set; }
    }
}
