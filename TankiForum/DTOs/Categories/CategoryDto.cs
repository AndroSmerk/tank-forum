namespace TankiForum.DTOs.Categories;

/// <summary>
/// Forum category — top-level grouping of discussion sections.
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Unique category identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short description of the category.
    /// </summary>
    public string? Description { get; set; }
}
