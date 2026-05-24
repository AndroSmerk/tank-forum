namespace TankiForum.DTOs.Sections;

/// <summary>
/// Forum section belonging to a category.
/// </summary>
public class SectionDto
{
    /// <summary>
    /// Unique section identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifier of the parent category.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Section name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short description of the section.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Name of the parent category.
    /// </summary>
    public string? CategoryName { get; set; }
}
