using Microsoft.EntityFrameworkCore;
using TankiForum.Data;
using TankiForum.DTOs.Common;
using TankiForum.DTOs.Topics;
using TankiForum.Models;
using TankiForum.Services.Interfaces;

namespace TankiForum.Services;

public class TopicService : ITopicService
{
    private readonly AppDbContext _context;

    public TopicService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TopicDto?> GetByIdAsync(int id)
    {
        var topic = await _context.Topics
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (topic is null) return null;

        var postCount = await _context.Posts.CountAsync(p => p.TopicId == id);

        return new TopicDto
        {
            Id = topic.Id,
            SectionId = topic.SectionId,
            UserId = topic.UserId,
            Username = topic.User.Username,
            Avatar = topic.User.Avatar,
            Title = topic.Title,
            Tags = topic.Tags,
            CreatedAt = topic.CreatedAt,
            UpdatedAt = topic.UpdatedAt,
            PostCount = postCount
        };
    }

    public async Task<PaginatedResponse<TopicListDto>> GetAllAsync(int? categoryId, int? sectionId, string? search, int page, int pageSize)
    {
        var query = _context.Topics
            .Include(t => t.User)
            .Include(t => t.Section)
                .ThenInclude(s => s.Category)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(t => t.Section.CategoryId == categoryId.Value);

        if (sectionId.HasValue)
            query = query.Where(t => t.SectionId == sectionId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Title.Contains(search));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TopicListDto
            {
                Id = t.Id,
                SectionId = t.SectionId,
                CategoryId = t.Section.CategoryId,
                CategoryName = t.Section.Category.Name,
                UserId = t.UserId,
                Username = t.User.Username,
                Avatar = t.User.Avatar,
                Title = t.Title,
                Tags = t.Tags,
                PostCount = t.Posts.Count,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return new PaginatedResponse<TopicListDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TopicDto> CreateAsync(int userId, CreateTopicRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null)
            throw new UnauthorizedAccessException("User not found.");
        if (user.IsBanned)
            throw new InvalidOperationException("Ваш аккаунт заблокирован. Создание тем недоступно.");

        var sectionExists = await _context.ForumSections.AnyAsync(s => s.Id == request.SectionId);
        if (!sectionExists)
            throw new InvalidOperationException("Section not found.");

        var topic = new Topic
        {
            SectionId = request.SectionId,
            UserId = userId,
            Title = request.Title,
            Tags = request.Tags
        };

        var post = new Post
        {
            TopicId = topic.Id,
            UserId = userId,
            Content = request.Content
        };
        topic.Posts.Add(post);

        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();

        var topicAuthor = await _context.Users.FindAsync(userId);

        return new TopicDto
        {
            Id = topic.Id,
            SectionId = topic.SectionId,
            UserId = topic.UserId,
            Username = topicAuthor!.Username,
            Avatar = topicAuthor.Avatar,
            Title = topic.Title,
            Tags = topic.Tags,
            CreatedAt = topic.CreatedAt,
            UpdatedAt = topic.UpdatedAt,
            PostCount = 1
        };
    }

    public async Task<TopicDto?> UpdateAsync(int id, int userId, UpdateTopicRequest request)
    {
        var topic = await _context.Topics
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (topic is null) return null;

        if (topic.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own topics.");

        topic.Title = request.Title;
        topic.Tags = request.Tags;
        topic.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new TopicDto
        {
            Id = topic.Id,
            SectionId = topic.SectionId,
            UserId = topic.UserId,
            Username = topic.User.Username,
            Avatar = topic.User.Avatar,
            Title = topic.Title,
            Tags = topic.Tags,
            CreatedAt = topic.CreatedAt,
            UpdatedAt = topic.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var topic = await _context.Topics.FindAsync(id);
        if (topic is null) return false;

        _context.Topics.Remove(topic);
        await _context.SaveChangesAsync();
        return true;
    }
}
