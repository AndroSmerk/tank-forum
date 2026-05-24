using Microsoft.EntityFrameworkCore;
using TankiForum.Data;
using TankiForum.DTOs.Messages;
using TankiForum.Models;
using TankiForum.Services.Interfaces;

namespace TankiForum.Services;

public class MessageService : IMessageService
{
    private readonly AppDbContext _context;

    public MessageService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MessageDto> SendAsync(int senderId, SendMessageRequest request)
    {
        var receiverExists = await _context.Users.AnyAsync(u => u.Id == request.ReceiverId);
        if (!receiverExists)
            throw new InvalidOperationException("Receiver not found.");

        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            Subject = request.Subject,
            Content = request.Content
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(senderId);
        var receiver = await _context.Users.FindAsync(request.ReceiverId);

        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderName = sender!.Username,
            ReceiverId = message.ReceiverId,
            ReceiverName = receiver!.Username,
            Subject = message.Subject,
            Content = message.Content,
            IsRead = message.IsRead,
            CreatedAt = message.CreatedAt
        };
    }

    public async Task<List<MessageDto>> GetInboxAsync(int userId)
    {
        return await _context.Messages
            .Where(m => m.ReceiverId == userId)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.Username,
                ReceiverId = m.ReceiverId,
                Subject = m.Subject,
                Content = m.Content,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<MessageDto>> GetSentAsync(int userId)
    {
        return await _context.Messages
            .Where(m => m.SenderId == userId)
            .Include(m => m.Receiver)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                ReceiverName = m.Receiver.Username,
                Subject = m.Subject,
                Content = m.Content,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<MessageDto?> GetByIdAsync(int id, int userId)
    {
        var message = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .FirstOrDefaultAsync(m => m.Id == id && (m.SenderId == userId || m.ReceiverId == userId));

        if (message is null) return null;

        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderName = message.Sender.Username,
            ReceiverId = message.ReceiverId,
            ReceiverName = message.Receiver.Username,
            Subject = message.Subject,
            Content = message.Content,
            IsRead = message.IsRead,
            CreatedAt = message.CreatedAt
        };
    }

    public async Task<bool> MarkAsReadAsync(int id, int userId)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == id && m.ReceiverId == userId);

        if (message is null) return false;

        message.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
