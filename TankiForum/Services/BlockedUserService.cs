using Microsoft.EntityFrameworkCore;
using TankiForum.Data;
using TankiForum.DTOs.Users;
using TankiForum.Models;
using TankiForum.Services.Interfaces;

namespace TankiForum.Services;

public class BlockedUserService : IBlockedUserService
{
    private readonly AppDbContext _context;

    public BlockedUserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task BlockAsync(int userId, int blockedUserId)
    {
        if (userId == blockedUserId)
            throw new InvalidOperationException("Cannot block yourself.");

        var exists = await _context.BlockedUsers.AnyAsync(b =>
            b.UserId == userId && b.BlockedUserId == blockedUserId);

        if (exists) return;

        var blockedUser = await _context.Users.FindAsync(blockedUserId);
        if (blockedUser is null)
            throw new InvalidOperationException("User not found.");

        _context.BlockedUsers.Add(new BlockedUser
        {
            UserId = userId,
            BlockedUserId = blockedUserId
        });

        blockedUser.IsBanned = true;
        blockedUser.BannedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task UnblockAsync(int userId, int blockedUserId)
    {
        var entry = await _context.BlockedUsers.FirstOrDefaultAsync(b =>
            b.BlockedUserId == blockedUserId);

        if (entry is null) return;

        var blockedUser = await _context.Users.FindAsync(blockedUserId);
        if (blockedUser is not null)
        {
            blockedUser.IsBanned = false;
            blockedUser.BannedAt = null;
            blockedUser.BanReason = null;
        }

        _context.BlockedUsers.Remove(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<List<BlockedUserDto>> GetBlockedAsync(int userId = 0)
    {
        return await _context.BlockedUsers
            .Include(b => b.Blocked)
            .OrderByDescending(b => b.BlockedAt)
            .Select(b => new BlockedUserDto
            {
                UserId = b.BlockedUserId,
                Username = b.Blocked.Username,
                Avatar = b.Blocked.Avatar,
                BlockedAt = b.BlockedAt
            })
            .ToListAsync();
    }

    public async Task<bool> IsBlockedAsync(int userId, int targetUserId)
    {
        return await _context.BlockedUsers.AnyAsync(b =>
            b.UserId == userId && b.BlockedUserId == targetUserId);
    }
}
