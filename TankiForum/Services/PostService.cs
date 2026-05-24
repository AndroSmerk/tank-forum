using Microsoft.EntityFrameworkCore;
using TankiForum.Data;
using TankiForum.DTOs.Posts;
using TankiForum.Models;
using TankiForum.Services.Interfaces;

namespace TankiForum.Services;

public class PostService : IPostService
{
    private readonly AppDbContext _context;

    public PostService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PostDto>> GetByTopicIdAsync(int topicId)
    {
        var posts = await _context.Posts
            .Where(p => p.TopicId == topicId)
            .Include(p => p.User)
            .Include(p => p.Likes)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            TopicId = p.TopicId,
            UserId = p.UserId,
            Username = p.User.Username,
            Avatar = p.User.Avatar,
            Content = p.User.IsBanned ? null : p.Content,
            QuotePostId = p.QuotePostId,
            LikeCount = p.Likes?.Count ?? 0,
            IsBannedAuthor = p.User.IsBanned,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();
    }

    public async Task<PostDto> CreateAsync(int userId, CreatePostRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null)
            throw new UnauthorizedAccessException("User not found.");
        if (user.IsBanned)
            throw new InvalidOperationException("Ваш аккаунт заблокирован. Отправка сообщений недоступна.");

        var topicExists = await _context.Topics.AnyAsync(t => t.Id == request.TopicId);
        if (!topicExists)
            throw new InvalidOperationException("Topic not found.");

        if (request.QuotePostId.HasValue && request.QuotePostId.Value > 0)
        {
            var quoteExists = await _context.Posts.AnyAsync(p => p.Id == request.QuotePostId.Value);
            if (!quoteExists)
                throw new InvalidOperationException("Quoted post not found.");
        }

        var post = new Post
        {
            TopicId = request.TopicId,
            UserId = userId,
            Content = request.Content,
            QuotePostId = request.QuotePostId
        };

        _context.Posts.Add(post);

        var topic = await _context.Topics.FindAsync(request.TopicId);
        if (topic is not null)
            topic.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new PostDto
        {
            Id = post.Id,
            TopicId = post.TopicId,
            UserId = post.UserId,
            Username = user.Username,
            Avatar = user.Avatar,
            Content = post.Content,
            QuotePostId = post.QuotePostId,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    public async Task<PostDto?> UpdateAsync(int id, int userId, UpdatePostRequest request)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null) return null;

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own posts.");

        post.Content = request.Content;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new PostDto
        {
            Id = post.Id,
            TopicId = post.TopicId,
            UserId = post.UserId,
            Username = post.User.Username,
            Avatar = post.User.Avatar,
            Content = post.Content,
            QuotePostId = post.QuotePostId,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post is null) return false;

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LikePostAsync(int postId, int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return false;
        if (user.IsBanned) return false;

        var post = await _context.Posts.FindAsync(postId);
        if (post is null) return false;

        var postAuthor = await _context.Users.FindAsync(post.UserId);
        if (postAuthor is null) return false;

        var existingLike = await _context.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

        if (existingLike is not null)
        {
            _context.PostLikes.Remove(existingLike);
            if (postAuthor.Respects > 0) postAuthor.Respects--;
            await _context.SaveChangesAsync();
            return false;
        }

        _context.PostLikes.Add(new PostLike
        {
            PostId = postId,
            UserId = userId
        });

        postAuthor.Respects++;
        await _context.SaveChangesAsync();
        return true;
    }
}
