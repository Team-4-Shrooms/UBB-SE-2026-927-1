using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/transactions")]
public sealed class TransactionEndpointsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IUserService _userService;
    private readonly IMovieService _movieService;
    private readonly IEquipmentService _equipmentService;
    private readonly IEventService _eventService;

    public TransactionEndpointsController(
        ITransactionService transactionService,
        IUserService userService,
        IMovieService movieService,
        IEquipmentService equipmentService,
        IEventService eventService)
    {
        _transactionService = transactionService;
        _userService = userService;
        _movieService = movieService;
        _equipmentService = equipmentService;
        _eventService = eventService;
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetTransactionsByUserId(int userId, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
    {
        var transactions = await _transactionService.GetTransactionsByUserIdAsync(userId, page, pageSize);
        return Ok(transactions.Select(transaction => transaction.ToDto()));
    }

    [HttpPost]
    public async Task<IActionResult> LogTransaction([FromBody] LogTransactionRequestBody body)
    {
        var buyer = await _userService.GetUserByIdAsync(body.BuyerId)
            ?? throw new InvalidOperationException($"User {body.BuyerId} not found.");

        var transaction = new Transaction
        {
            Amount = body.Amount,
            Type = body.Type ?? string.Empty,
            Status = body.Status ?? string.Empty,
            Timestamp = body.Timestamp,
            ShippingAddress = body.ShippingAddress,
            Buyer = buyer,
            Seller = body.SellerId.HasValue ? await _userService.GetUserByIdAsync(body.SellerId.Value) : null,
            Equipment = body.EquipmentId.HasValue ? await _equipmentService.GetEquipmentByIdAsync(body.EquipmentId.Value) : null,
            Movie = body.MovieId.HasValue ? await _movieService.GetMovieByIdAsync(body.MovieId.Value) : null,
            Event = body.EventId.HasValue ? await _eventService.GetEventByIdAsync(body.EventId.Value) : null,
        };
        await _transactionService.LogTransactionAsync(transaction);
        return Ok();
    }

    [HttpPut("{transactionId:int}/status")]
    public async Task<IActionResult> UpdateTransactionStatus(int transactionId, [FromBody] UpdateTransactionStatusRequestBody body)
    {
        await _transactionService.UpdateTransactionStatusAsync(transactionId, body.NewStatus);
        return Ok();
    }
}
