using TankiForum.DTOs.Clans;

namespace TankiForum.Services.Interfaces;

public interface IClanService
{
    Task<List<ClanDto>> GetAllAsync(int? currentUserId = null);
    Task<ClanDto> CreateAsync(int userId, CreateClanRequest request);
    Task<bool> DeleteAsync(int clanId);
    Task<bool> JoinAsync(int userId, int clanId);
    Task<bool> LeaveAsync(int userId, int clanId);
}
