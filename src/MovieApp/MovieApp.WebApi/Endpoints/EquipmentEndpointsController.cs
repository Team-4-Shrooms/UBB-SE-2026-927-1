using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/equipment")]
public sealed class EquipmentEndpointsController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentEndpointsController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet("available")]
    public async Task<IActionResult> FetchAvailableEquipment()
    {
        return Ok((await _equipmentService.GetAvailableEquipmentAsync()).Select(equipment => equipment.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
        return Ok(equipment?.ToDto());
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] EquipmentListItemRequestBody body)
    {
        var equipment = body.ToModel();
        await _equipmentService.ListItemAsync(equipment);
        return Ok();
    }

    [HttpPost("{id:int}/purchase")]
    public async Task<IActionResult> PurchaseAsync(int id, [FromBody] PurchaseEquipmentRequestBody body)
    {
        try
        {
            await _equipmentService.PurchaseEquipmentAsync(id, body.BuyerId, body.Price, body.Address);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred: " + ex.Message);
        }
    }
}
