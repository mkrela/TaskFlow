using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Services.Users;
using TaskFlow.Application.DTOs.Users;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("by-email")]
    public async Task<ActionResult<UserDto>> GetByEmail([FromQuery] string email, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByEmailAsync(email, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateAsync(id, request, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }
}