namespace TankiForum.DTOs.Topics;

/// <summary>
/// Forum topic (thread) within a section.
/// </summary>
public class TopicDto
{
    /// <summary>
    /// Unique topic identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifier of the parent section.
    /// </summary>
    public int SectionId { get; set; }

    /// <summary>
    /// Identifier of the topic author.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Username of the topic author.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Avatar URL of the topic author.
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// Topic title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated tags.
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Total number of posts in this topic.
    /// </summary>
    public int PostCount { get; set; }
}
