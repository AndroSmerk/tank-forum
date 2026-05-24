using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Topics;

/// <summary>
/// Request payload for updating a topic's title.
/// </summary>
public class UpdateTopicRequest
{
    /// <summary>
    /// New topic title (max 200 characters).
    /// </summary>
    [Required, MinLength(1), MaxLength(200)]
    public string Title { get; set; } = string.Empty;
}
