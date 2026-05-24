using TankiForum.DTOs.Users;

namespace TankiForum.Services.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(int id);
    Task<List<UserDto>> SearchAsync(string query);
    Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request, IFormFile? avatarFile = null);
    Task<UserStatsDto?> GetStatsAsync(int id);
    Task HeartbeatAsync(int userId);
    Task<List<UserDto>> GetOnlineAsync();
    Task<List<string>> GetUserClansAsync(int userId);
    Task<GlobalStatsDto> GetGlobalStatsAsync();
}
