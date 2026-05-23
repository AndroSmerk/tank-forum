using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Topics;

/// <summary>
/// Request payload for creating a new topic (includes the first post).
/// </summary>
public class CreateTopicRequest
{
    /// <summary>
    /// Identifier of the parent section.
    /// </summary>
    [Required]
    public int SectionId { get; set; }

    /// <summary>
    /// Topic title (max 200 characters).
    /// </summary>
    [Required, MinLength(1), MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Content of the first post in the topic.
    /// </summary>
    [Required, MaxLength(10000)]
    public string Content { get; set; } = string.Empty;
}
