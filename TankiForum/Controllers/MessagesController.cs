using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankiForum.DTOs.Messages;
using TankiForum.Services.Interfaces;

namespace TankiForum.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("inbox")]
    public async Task<IActionResult> GetInbox()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var messages = await _messageService.GetInboxAsync(userId);
        return Ok(messages);
    }

    [HttpGet("sent")]
    public async Task<IActionResult> GetSent()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var messages = await _messageService.GetSentAsync(userId);
        return Ok(messages);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var message = await _messageService.GetByIdAsync(id, userId);
        if (message is null) return NotFound();
        return Ok(message);
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var message = await _messageService.SendAsync(userId, request);
        return Ok(message);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _messageService.MarkAsReadAsync(id, userId);
        if (!result) return NotFound();
        return NoContent();
    }
}
