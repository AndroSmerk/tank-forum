namespace TankiForum.DTOs.Users;

public class BlockedUserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public DateTime BlockedAt { get; set; }
}
