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
}
