namespace TankiForum.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
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
    public string Role { get; set; } = "User";
    public bool IsBanned { get; set; }
    public DateTime? BannedAt { get; set; }
    public string? BanReason { get; set; }
    public int Respects { get; set; }
    public string? Achievements { get; set; }
    public string? Medals { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Topic> CreatedThreads { get; set; } = new List<Topic>();
    public ICollection<Post> CreatedPosts { get; set; } = new List<Post>();
    public ICollection<UserClan> Clans { get; set; } = new List<UserClan>();
    public ICollection<BlockedUser> BlockedUsers { get; set; } = new List<BlockedUser>();
}
