using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankiForum.DTOs.Topics;
using TankiForum.Services.Interfaces;

namespace TankiForum.Controllers;

/// <summary>
/// Manage forum topics (threads) within sections.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TopicsController : ControllerBase
{
    private readonly ITopicService _topicService;

    public TopicsController(ITopicService topicService)
    {
        _topicService = topicService;
    }

    /// <summary>
    /// Get all topics with optional filtering, search, and pagination.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] int? sectionId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _topicService.GetAllAsync(categoryId, sectionId, search, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific topic by ID with post count.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var topic = await _topicService.GetByIdAsync(id);
        if (topic is null) return NotFound();
        return Ok(topic);
    }

    /// <summary>
    /// Create a new topic (requires authentication). The first post is created together with the topic.
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTopicRequest request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var topic = await _topicService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = topic.Id }, topic);
    }

    /// <summary>
    /// Update a topic's title (requires authentication).
    /// </summary>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTopicRequest request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var topic = await _topicService.UpdateAsync(id, userId, request);
        if (topic is null) return NotFound();
        return Ok(topic);
    }

    /// <summary>
    /// Delete a topic and all its posts (requires authentication).
    /// </summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _topicService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
