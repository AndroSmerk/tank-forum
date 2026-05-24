namespace TankiForum.Models;

public class Clan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserClan> UserClans { get; set; } = new List<UserClan>();
}
