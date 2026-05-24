namespace TankiForum.DTOs.Topics;

public class TopicListDto
{
    public int Id { get; set; }
    public int SectionId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public int PostCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
