using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Categories;

/// <summary>
/// Request payload for updating an existing forum category.
/// </summary>
public class UpdateCategoryRequest
{
    /// <summary>
    /// New category name (max 100 characters, required).
    /// </summary>
    [Required, MinLength(1), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// New description (max 500 characters).
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}
