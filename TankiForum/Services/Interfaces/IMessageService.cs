using TankiForum.DTOs.Messages;

namespace TankiForum.Services.Interfaces;

public interface IMessageService
{
    Task<MessageDto> SendAsync(int senderId, SendMessageRequest request);
    Task<List<MessageDto>> GetInboxAsync(int userId);
    Task<List<MessageDto>> GetSentAsync(int userId);
    Task<MessageDto?> GetByIdAsync(int id, int userId);
    Task<bool> MarkAsReadAsync(int id, int userId);
}
