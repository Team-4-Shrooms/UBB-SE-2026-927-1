using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.DTO.WebAPI;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/users")]
public sealed class UserEndpointsController : ControllerBase
{
    private readonly UserRepository _repository;

    public UserEndpointsController(UserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{userId:int}/balance")]
    public IActionResult GetBalance(int userId)
    {
        return Ok(_repository.GetBalance(userId));
    }

    [HttpPut("{userId:int}/balance")]
    public IActionResult UpdateBalance(int userId, [FromBody] UpdateBalanceRequestBody request)
    {
        _repository.UpdateBalance(userId, request.NewBalance);
        return Ok();
    }
}
