using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankiForum.DTOs.Users;
using TankiForum.Services.Interfaces;

namespace TankiForum.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class BlockedUsersController : ControllerBase
{
    private readonly IBlockedUserService _blockedUserService;

    public BlockedUsersController(IBlockedUserService blockedUserService)
    {
        _blockedUserService = blockedUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBlocked()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null) return Unauthorized();

        var blocked = await _blockedUserService.GetBlockedAsync(int.Parse(userIdClaim));
        return Ok(blocked);
    }

    [HttpPost("block")]
    public async Task<IActionResult> Block([FromBody] BlockUserRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null) return Unauthorized();

        await _blockedUserService.BlockAsync(int.Parse(userIdClaim), request.BlockedUserId);
        return Ok(new { message = "User blocked." });
    }

    [HttpPost("unblock")]
    public async Task<IActionResult> Unblock([FromBody] BlockUserRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null) return Unauthorized();

        await _blockedUserService.UnblockAsync(int.Parse(userIdClaim), request.BlockedUserId);
        return Ok(new { message = "User unblocked." });
    }
}
