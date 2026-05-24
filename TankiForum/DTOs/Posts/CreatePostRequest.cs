using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Posts;

/// <summary>
/// Request payload for creating a new post in a topic.
/// </summary>
public class CreatePostRequest
{
    /// <summary>
    /// Identifier of the parent topic.
    /// </summary>
    [Required]
    public int TopicId { get; set; }

    /// <summary>
    /// Post content (supports markdown/BBcode).
    /// </summary>
    [Required, MaxLength(10000)]
    public string Content { get; set; } = string.Empty;

    public int? QuotePostId { get; set; }
}
