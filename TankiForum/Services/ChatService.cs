using Microsoft.EntityFrameworkCore;
using TankiForum.Data;
using TankiForum.DTOs.Chat;
using TankiForum.Models;
using TankiForum.Services.Interfaces;

namespace TankiForum.Services;

public class ChatService : IChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ChatMessageDto> SendMessageAsync(int userId, string content)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null)
            throw new UnauthorizedAccessException("User not found.");
        if (user.IsBanned)
            throw new InvalidOperationException("Ваш аккаунт заблокирован. Чат недоступен.");

        var message = new ChatMessage
        {
            UserId = userId,
            Content = content
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        return new ChatMessageDto
        {
            Id = message.Id,
            UserId = message.UserId,
            Username = user.Username,
            Avatar = user.Avatar,
            RankClass = user.RankClass,
            Content = message.Content,
            CreatedAt = message.CreatedAt
        };
    }

    public async Task<List<ChatMessageDto>> GetRecentMessagesAsync(int limit = 50)
    {
        return await _context.ChatMessages
            .Include(m => m.User)
            .OrderByDescending(m => m.CreatedAt)
            .Take(limit)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatMessageDto
            {
                Id = m.Id,
                UserId = m.UserId,
                Username = m.User.Username,
                Avatar = m.User.Avatar,
                RankClass = m.User.RankClass,
                Content = m.User.IsBanned ? null! : m.Content,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();
    }
}
