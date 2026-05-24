namespace TankiForum.DTOs.Posts;

public class PostDto
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? QuotePostId { get; set; }
    public int LikeCount { get; set; }
    public bool IsBannedAuthor { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
