using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankiForum.DTOs.Posts;
using TankiForum.Services.Interfaces;

namespace TankiForum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByTopic([FromQuery] int topicId)
    {
        var posts = await _postService.GetByTopicIdAsync(topicId);
        return Ok(posts);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var post = await _postService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetByTopic), new { topicId = post.TopicId }, post);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var post = await _postService.UpdateAsync(id, userId, request);
        if (post is null) return NotFound();
        return Ok(post);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _postService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/like")]
    public async Task<IActionResult> Like(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var liked = await _postService.LikePostAsync(id, userId);
        return Ok(new { liked });
    }
}
