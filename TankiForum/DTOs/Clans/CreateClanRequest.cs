using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Clans;

/// <summary>
/// Request payload for creating a new clan.
/// </summary>
public class CreateClanRequest
{
    /// <summary>
    /// Clan name (max 100 characters, must be unique).
    /// </summary>
    [Required, MinLength(3), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional clan description (max 500 characters).
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}
