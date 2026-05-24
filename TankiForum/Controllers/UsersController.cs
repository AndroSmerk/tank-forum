using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankiForum.DTOs.Users;
using TankiForum.Services.Interfaces;

namespace TankiForum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null) return NotFound();
        return Ok(user);
    }

    [HttpGet("{id}/stats")]
    public async Task<IActionResult> GetStats(int id)
    {
        var stats = await _userService.GetStatsAsync(id);
        if (stats is null) return NotFound();
        return Ok(stats);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(new List<UserDto>());
        var users = await _userService.SearchAsync(q);
        return Ok(users);
    }

    [Authorize]
    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateUserRequest request, IFormFile? AvatarFile)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || int.Parse(userIdClaim) != id)
            return Forbid();

        var user = await _userService.UpdateAsync(id, request, AvatarFile);
        if (user is null) return NotFound();
        return Ok(user);
    }

    [Authorize]
    [HttpPost("heartbeat")]
    public async Task<IActionResult> Heartbeat()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null) return Unauthorized();

        await _userService.HeartbeatAsync(int.Parse(userIdClaim));
        return Ok();
    }

    [HttpGet("online")]
    public async Task<IActionResult> GetOnline()
    {
        var users = await _userService.GetOnlineAsync();
        return Ok(users);
    }

    [HttpGet("{id}/clans")]
    public async Task<IActionResult> GetUserClans(int id)
    {
        var clans = await _userService.GetUserClansAsync(id);
        return Ok(clans);
    }
}
