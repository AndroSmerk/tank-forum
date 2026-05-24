using TankiForum.DTOs.Chat;

namespace TankiForum.Services.Interfaces;

public interface IChatService
{
    Task<ChatMessageDto> SendMessageAsync(int userId, string content);
    Task<List<ChatMessageDto>> GetRecentMessagesAsync(int limit = 50);
}
