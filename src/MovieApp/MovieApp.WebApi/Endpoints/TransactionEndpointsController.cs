using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/transactions")]
public sealed class TransactionEndpointsController : ControllerBase
{
    private readonly TransactionRepository _repository;

    public TransactionEndpointsController(TransactionRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public IActionResult LogTransaction([FromBody] Transaction transaction)
    {
        try
        {
            _repository.LogTransaction(transaction);
            return Ok();
        }
        catch (ArgumentNullException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("users/{userId:int}")]
    public IActionResult GetTransactionsByUserId(int userId)
    {
        return Ok(_repository.GetTransactionsByUserId(userId));
    }

    [HttpPut("{transactionId:int}/status")]
    public IActionResult UpdateTransactionStatus(int transactionId, [FromBody] UpdateTransactionStatusRequest request)
    {
        _repository.UpdateTransactionStatus(transactionId, request.NewStatus);
        return Ok();
    }

    public sealed record UpdateTransactionStatusRequest(string NewStatus);
}
