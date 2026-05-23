using System.ComponentModel.DataAnnotations;

namespace TankiForum.DTOs.Users;

public class UpdateUserRequest
{
    [MaxLength(100)]
    public string? Vocation { get; set; }

    [Range(1, 120)]
    public int? Age { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(500)]
    public string? Quote { get; set; }

    [MaxLength(100)]
    public string? FavoriteTank { get; set; }
}
