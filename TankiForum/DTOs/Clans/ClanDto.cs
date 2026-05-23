namespace TankiForum.DTOs.Clans;

/// <summary>
/// Player clan information.
/// </summary>
public class ClanDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MemberCount { get; set; }
    public bool IsMember { get; set; }
}
