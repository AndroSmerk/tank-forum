namespace TankiForum.Models;

public class UserClan
{
    public int UserId { get; set; }
    public int ClanId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Clan Clan { get; set; } = null!;
}
