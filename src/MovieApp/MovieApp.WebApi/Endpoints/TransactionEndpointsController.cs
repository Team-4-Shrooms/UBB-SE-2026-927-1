using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.DTO.WebAPI;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/transactions")]
public sealed class TransactionEndpointsController : ControllerBase
{
    private readonly TransactionRepository _repository;
    private readonly IMovieAppDbContext _context;

    public TransactionEndpointsController(TransactionRepository repository, IMovieAppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet("users/{userId:int}")]
    public IActionResult GetTransactionsByUserId(int userId)
    {
        return Ok(_repository.GetTransactionsByUserId(userId).Select(transaction => transaction.ToDto()));
    }
}
