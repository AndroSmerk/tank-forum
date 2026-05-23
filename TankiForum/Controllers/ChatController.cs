using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankiForum.DTOs.Chat;
using TankiForum.Services.Interfaces;

namespace TankiForum.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRecent([FromQuery] int limit = 50)
    {
        var messages = await _chatService.GetRecentMessagesAsync(limit);
        return Ok(messages);
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendChatMessageRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null) return Unauthorized();

        var message = await _chatService.SendMessageAsync(int.Parse(userIdClaim), request.Content);
        return Ok(message);
    }
}
