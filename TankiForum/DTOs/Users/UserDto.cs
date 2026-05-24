namespace TankiForum.DTOs.Users;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Vocation { get; set; }
    public int? Age { get; set; }
    public string? City { get; set; }
    public string? DisplayName { get; set; }
    public string? Rank { get; set; }
    public string? RankClass { get; set; }
    public string? Quote { get; set; }
    public string? FavoriteTank { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public int Respects { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsBanned { get; set; }
    public DateTime CreatedAt { get; set; }
}
