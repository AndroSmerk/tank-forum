namespace TankiForum.Models;

public class Post
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? QuotePostId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Topic Topic { get; set; } = null!;
    public User User { get; set; } = null!;
    public Post? QuotePost { get; set; }
    public ICollection<Post>? QuotedByPosts { get; set; }
    public ICollection<PostLike>? Likes { get; set; }
}
