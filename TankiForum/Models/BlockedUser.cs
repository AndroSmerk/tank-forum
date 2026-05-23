namespace TankiForum.Models;

public class BlockedUser
{
    public int UserId { get; set; }
    public int BlockedUserId { get; set; }
    public DateTime BlockedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public User Blocked { get; set; } = null!;
}
