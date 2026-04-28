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

    [HttpPost]
    public async Task<IActionResult> LogTransaction([FromBody] LogTransactionRequestBody transaction)
    {
        try
        {
            User buyer = await _context.Users.FindAsync(transaction.BuyerId)
                ?? throw new InvalidOperationException($"User {transaction.BuyerId} not found.");

            User? seller = null;
            if (transaction.SellerId.HasValue)
            {
                seller = await _context.Users.FindAsync(transaction.SellerId.Value)
                    ?? throw new InvalidOperationException($"User {transaction.SellerId.Value} not found.");
            }

            Transaction loggedTransaction = new Transaction
            {
                Amount = transaction.Amount,
                Type = transaction.Type,
                Status = transaction.Status,
                Timestamp = transaction.Timestamp,
                ShippingAddress = transaction.ShippingAddress,
                Buyer = buyer,
                Seller = seller,
                Equipment = transaction.EquipmentId.HasValue
                    ? await _context.Equipment.FindAsync(transaction.EquipmentId.Value)
                    : null,
                Movie = transaction.MovieId.HasValue
                    ? await _context.Movies.FindAsync(transaction.MovieId.Value)
                    : null,
                Event = transaction.EventId.HasValue
                    ? await _context.MovieEvents.FindAsync(transaction.EventId.Value)
                    : null,
            };

            _repository.LogTransaction(loggedTransaction);
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

    [HttpGet("users/{userId:int}")]
    public IActionResult GetTransactionsByUserId(int userId)
    {
        return Ok(_repository.GetTransactionsByUserId(userId).Select(transaction => transaction.ToDto()));
    }

    [HttpPut("{transactionId:int}/status")]
    public IActionResult UpdateTransactionStatus(int transactionId, [FromBody] UpdateTransactionStatusRequestBody request)
    {
        _repository.UpdateTransactionStatus(transactionId, request.NewStatus);
        return Ok();
    }
}
