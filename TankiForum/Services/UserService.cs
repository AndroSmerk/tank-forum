using Microsoft.EntityFrameworkCore;
using TankiForum.Data;
using TankiForum.DTOs.Users;
using TankiForum.Services.Interfaces;

namespace TankiForum.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public UserService(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Avatar = user.Avatar,
            Vocation = user.Vocation,
            Age = user.Age,
            City = user.City,
            DisplayName = user.DisplayName,
            Rank = user.Rank,
            RankClass = user.RankClass,
            Quote = user.Quote,
            FavoriteTank = user.FavoriteTank,
            LastActivityAt = user.LastActivityAt,
            Respects = user.Respects,
            Achievements = user.Achievements,
            Medals = user.Medals,
            Role = user.Role,
            IsBanned = user.IsBanned,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserStatsDto?> GetStatsAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return null;

        var topicCount = await _context.Topics.CountAsync(t => t.UserId == id);
        var postCount = await _context.Posts.CountAsync(p => p.UserId == id);
        var likeCount = await _context.PostLikes.CountAsync(pl => pl.User.Username == user.Username || _context.Posts.Where(p => p.Id == pl.PostId).Select(p => p.UserId).FirstOrDefault() == id);

        return new UserStatsDto
        {
            TopicCount = topicCount,
            PostCount = postCount,
            RespectCount = user.Respects,
            LikeCount = likeCount,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task HeartbeatAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is not null)
        {
            user.LastActivityAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<UserDto>> GetOnlineAsync()
    {
        var threshold = DateTime.UtcNow.AddMinutes(-5);
        return await _context.Users
            .Where(u => u.LastActivityAt.HasValue && u.LastActivityAt > threshold)
            .OrderByDescending(u => u.LastActivityAt)
            .Take(50)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Avatar = u.Avatar,
                Vocation = u.Vocation,
                Age = u.Age,
                City = u.City,
                DisplayName = u.DisplayName,
                Rank = u.Rank,
                RankClass = u.RankClass,
                Quote = u.Quote,
                FavoriteTank = u.FavoriteTank,
                LastActivityAt = u.LastActivityAt,
                Respects = u.Respects,
                Achievements = u.Achievements,
                Medals = u.Medals,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<string>> GetUserClansAsync(int userId)
    {
        return await _context.UserClans
            .Where(uc => uc.UserId == userId)
            .Include(uc => uc.Clan)
            .Select(uc => uc.Clan.Name)
            .ToListAsync();
    }

    public async Task<List<UserDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<UserDto>();

        return await _context.Users
            .Where(u => u.Username.Contains(query))
            .Take(20)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Avatar = u.Avatar,
                Vocation = u.Vocation,
                Age = u.Age,
                City = u.City,
                DisplayName = u.DisplayName,
                Rank = u.Rank,
                RankClass = u.RankClass,
                Quote = u.Quote,
                FavoriteTank = u.FavoriteTank,
                Respects = u.Respects,
                Achievements = u.Achievements,
                Medals = u.Medals,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request, IFormFile? avatarFile = null)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return null;

        user.Vocation = request.Vocation ?? user.Vocation;
        user.Age = request.Age ?? user.Age;
        user.City = request.City ?? user.City;
        user.Quote = request.Quote ?? user.Quote;
        user.FavoriteTank = request.FavoriteTank ?? user.FavoriteTank;

        if (avatarFile is not null && avatarFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Недопустимый формат файла. Разрешены: JPG, PNG, GIF, WebP.");

            if (avatarFile.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("Файл не должен превышать 5MB.");

            var uploadsDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "avatars");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }

            user.Avatar = $"/uploads/avatars/{fileName}";
        }

        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Avatar = user.Avatar,
            Vocation = user.Vocation,
            Age = user.Age,
            City = user.City,
            DisplayName = user.DisplayName,
            Rank = user.Rank,
            RankClass = user.RankClass,
            Quote = user.Quote,
            FavoriteTank = user.FavoriteTank,
            Respects = user.Respects,
            Achievements = user.Achievements,
            Medals = user.Medals,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
