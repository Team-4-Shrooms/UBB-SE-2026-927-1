using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs;
using MovieApp.WebApi.Mappings;
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
    public async Task<IActionResult> FetchAvailableEquipment()
    {
        return Ok((await _repository.FetchAvailableEquipmentAsync()).Select(equipment => equipment.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var equipment = await _repository.GetByIdAsync(id);
        return Ok(equipment?.ToDto());
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] EquipmentListItemRequestBody body)
    {
        var equipment = body.ToModel();
        equipment.Status = EquipmentStatus.Available;
        equipment.Seller = await _context.Users.FindAsync(body.SellerId)
            ?? throw new InvalidOperationException($"User {body.SellerId} not found.");
        await _repository.AddAsync(equipment);
        await _repository.SaveChangesAsync();
        return Ok();
    }
}
