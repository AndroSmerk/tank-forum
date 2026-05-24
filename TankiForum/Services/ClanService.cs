using Microsoft.EntityFrameworkCore;
using TankiForum.Data;
using TankiForum.DTOs.Clans;
using TankiForum.Models;
using TankiForum.Services.Interfaces;
using Npgsql;

namespace TankiForum.Services;

public class ClanService : IClanService
{
    private readonly AppDbContext _context;

    public ClanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClanDto>> GetAllAsync(int? currentUserId = null)
    {
        var clans = await _context.Clans
            .Select(c => new ClanDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                MemberCount = c.UserClans.Count
            })
            .ToListAsync();

        if (currentUserId.HasValue)
        {
            var userClanIds = await _context.UserClans
                .Where(uc => uc.UserId == currentUserId.Value)
                .Select(uc => uc.ClanId)
                .ToListAsync();

            foreach (var clan in clans)
            {
                clan.IsMember = userClanIds.Contains(clan.Id);
            }
        }

        return clans;
    }

    public async Task<ClanDto> CreateAsync(int userId, CreateClanRequest request)
    {
        if (await _context.Clans.AnyAsync(c => c.Name == request.Name))
            throw new InvalidOperationException("Clan name already taken.");

        var clan = new Clan
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Clans.Add(clan);
        await _context.SaveChangesAsync();

        var userClan = new UserClan
        {
            UserId = userId,
            ClanId = clan.Id
        };

        _context.UserClans.Add(userClan);
        await _context.SaveChangesAsync();

        return new ClanDto
        {
            Id = clan.Id,
            Name = clan.Name,
            Description = clan.Description,
            CreatedAt = clan.CreatedAt,
            MemberCount = 1
        };
    }

    public async Task<bool> DeleteAsync(int clanId)
    {
        var clan = await _context.Clans.FindAsync(clanId);
        if (clan is null) return false;

        _context.Clans.Remove(clan);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> JoinAsync(int userId, int clanId)
    {
        var clanExists = await _context.Clans.AnyAsync(c => c.Id == clanId);
        if (!clanExists) return false;

        var alreadyMember = await _context.UserClans
            .AnyAsync(uc => uc.UserId == userId && uc.ClanId == clanId);
        if (alreadyMember) return false;

        var userClan = new UserClan
        {
            UserId = userId,
            ClanId = clanId
        };

        _context.UserClans.Add(userClan);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            return false;
        }
        return true;
    }

    public async Task<bool> LeaveAsync(int userId, int clanId)
    {
        var userClan = await _context.UserClans
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ClanId == clanId);

        if (userClan is null) return false;

        _context.UserClans.Remove(userClan);
        await _context.SaveChangesAsync();
        return true;
    }

}
