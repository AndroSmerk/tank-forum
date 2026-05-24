using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Posts;

/// <summary>
/// Request payload for updating a post's content.
/// </summary>
public class UpdatePostRequest
{
    /// <summary>
    /// New post content.
    /// </summary>
    [Required, MaxLength(10000)]
    public string Content { get; set; } = string.Empty;
}
