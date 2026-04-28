using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.DTO.WebAPI;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/equipment")]
public sealed class EquipmentEndpointsController : ControllerBase
{
    private readonly EquipmentRepository _repository;
    private readonly IMovieAppDbContext _context;

    public EquipmentEndpointsController(EquipmentRepository repository, IMovieAppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet("available")]
    public IActionResult FetchAvailableEquipment()
    {
        return Ok(_repository.FetchAvailableEquipment().Select(equipment => equipment.ToDto()));
    }

    [HttpPost]
    public async Task<IActionResult> ListItem([FromBody] EquipmentListItemRequestBody item)
    {
        try
        {
            User seller = await _context.Users.FindAsync(item.SellerId)
                ?? throw new InvalidOperationException($"User {item.SellerId} not found.");

            Equipment equipment = new Equipment
            {
                Title = item.Title,
                Category = item.Category,
                Description = item.Description,
                Condition = item.Condition,
                Price = item.Price,
                ImageUrl = item.ImageUrl,
                Seller = seller,
            };

            _repository.ListItem(equipment);
            return Ok();
        }
        catch (ArgumentNullException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("{equipmentId:int}/purchase")]
    public IActionResult PurchaseEquipment(int equipmentId, [FromBody] PurchaseEquipmentRequestBody request)
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
}
