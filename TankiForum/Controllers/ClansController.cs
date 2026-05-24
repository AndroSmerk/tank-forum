using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankiForum.DTOs.Clans;
using TankiForum.Services.Interfaces;

namespace TankiForum.Controllers;

/// <summary>
/// Manage player clans — create, join, leave, and list clans.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClansController : ControllerBase
{
    private readonly IClanService _clanService;

    public ClansController(IClanService clanService)
    {
        _clanService = clanService;
    }

    /// <summary>
    /// Get all clans with member counts.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdClaim is not null ? int.Parse(userIdClaim) : null;
        var clans = await _clanService.GetAllAsync(userId);
        return Ok(clans);
    }

    /// <summary>
    /// Create a new clan (requires authentication). The creator automatically becomes a member.
    /// </summary>
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateClanRequest request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var clan = await _clanService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetAll), new { id = clan.Id }, clan);
    }

    /// <summary>
    /// Delete a clan by ID (requires authentication).
    /// </summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] int clanId)
    {
        var result = await _clanService.DeleteAsync(clanId);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Join an existing clan (requires authentication).
    /// </summary>
    [Authorize]
    [HttpPost("join")]
    public async Task<IActionResult> Join([FromBody] JoinClanRequest request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _clanService.JoinAsync(userId, request.ClanId);
        if (!result) return BadRequest(new { message = "Unable to join clan." });
        return Ok(new { message = "Joined clan successfully." });
    }

    /// <summary>
    /// Leave a clan (requires authentication).
    /// </summary>
    [Authorize]
    [HttpPost("leave")]
    public async Task<IActionResult> Leave([FromBody] JoinClanRequest request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _clanService.LeaveAsync(userId, request.ClanId);
        if (!result) return BadRequest(new { message = "Unable to leave clan." });
        return Ok(new { message = "Left clan successfully." });
    }
}
