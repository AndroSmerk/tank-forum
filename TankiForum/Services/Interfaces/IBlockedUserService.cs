using TankiForum.DTOs.Users;

namespace TankiForum.Services.Interfaces;

public interface IBlockedUserService
{
    Task BlockAsync(int userId, int blockedUserId);
    Task UnblockAsync(int userId, int blockedUserId);
    Task<List<BlockedUserDto>> GetBlockedAsync(int userId = 0);
    Task<bool> IsBlockedAsync(int userId, int targetUserId);
}
