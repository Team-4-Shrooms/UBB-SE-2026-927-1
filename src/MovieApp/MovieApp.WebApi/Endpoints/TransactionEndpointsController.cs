using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
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

    [HttpPost]
    public async Task<IActionResult> LogTransaction([FromBody] LogTransactionRequestBody body)
    {
        var transaction = new Transaction
        {
            Amount = body.Amount,
            Type = body.Type ?? string.Empty,
            Status = body.Status ?? string.Empty,
            Timestamp = body.Timestamp,
            ShippingAddress = body.ShippingAddress,
            Buyer = await _context.Users.FindAsync(body.BuyerId)
                ?? throw new InvalidOperationException($"User {body.BuyerId} not found."),
            Seller = body.SellerId.HasValue ? await _context.Users.FindAsync(body.SellerId.Value) : null,
            Equipment = body.EquipmentId.HasValue ? await _context.Equipment.FindAsync(body.EquipmentId.Value) : null,
            Movie = body.MovieId.HasValue ? await _context.Movies.FindAsync(body.MovieId.Value) : null,
            Event = body.EventId.HasValue ? await _context.MovieEvents.FindAsync(body.EventId.Value) : null,
        };
        _repository.LogTransaction(transaction);
        return Ok();
    }

    [HttpPut("{transactionId:int}/status")]
    public IActionResult UpdateTransactionStatus(int transactionId, [FromBody] UpdateTransactionStatusRequestBody body)
    {
        _repository.UpdateTransactionStatus(transactionId, body.NewStatus);
        return Ok();
    }
}
