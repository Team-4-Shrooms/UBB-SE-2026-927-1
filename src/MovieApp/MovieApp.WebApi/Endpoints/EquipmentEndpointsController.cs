using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/equipment")]
public sealed class EquipmentEndpointsController : ControllerBase
{
    private readonly EquipmentRepository _repository;

    public EquipmentEndpointsController(EquipmentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("available")]
    public IActionResult FetchAvailableEquipment()
    {
        return Ok(_repository.FetchAvailableEquipment());
    }

    [HttpPost]
    public IActionResult ListItem([FromBody] Equipment item)
    {
        try
        {
            _repository.ListItem(item);
            return Ok();
        }
        catch (ArgumentNullException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("{equipmentId:int}/purchase")]
    public IActionResult PurchaseEquipment(int equipmentId, [FromBody] PurchaseEquipmentRequest request)
    {
        try
        {
            _repository.PurchaseEquipment(equipmentId, request.BuyerId, request.Price, request.Address);
            return Ok();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    public sealed record PurchaseEquipmentRequest(int BuyerId, decimal Price, string Address);
}
