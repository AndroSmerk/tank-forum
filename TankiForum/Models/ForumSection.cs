namespace TankiForum.Models;

public class ForumSection
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ForumCategory Category { get; set; } = null!;
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
