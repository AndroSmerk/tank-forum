using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Sections;

/// <summary>
/// Request payload for creating a new forum section.
/// </summary>
public class CreateSectionRequest
{
    /// <summary>
    /// Identifier of the parent category (required).
    /// </summary>
    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// Section name (max 100 characters, required).
    /// </summary>
    [Required, MinLength(1), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description (max 500 characters).
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}
