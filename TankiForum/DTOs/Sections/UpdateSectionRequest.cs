using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Sections;

/// <summary>
/// Request payload for updating an existing forum section.
/// </summary>
public class UpdateSectionRequest
{
    /// <summary>
    /// New section name (max 100 characters, required).
    /// </summary>
    [Required, MinLength(1), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// New description (max 500 characters).
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}
