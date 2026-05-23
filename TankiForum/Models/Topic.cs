namespace TankiForum.Models;

public class Topic
{
    public int Id { get; set; }
    public int SectionId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ForumSection Section { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
