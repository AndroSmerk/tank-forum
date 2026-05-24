using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Categories;

/// <summary>
/// Request payload for creating a new forum category.
/// </summary>
public class CreateCategoryRequest
{
    /// <summary>
    /// Category name (max 100 characters, required).
    /// </summary>
    [Required, MinLength(1), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description (max 500 characters).
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}
