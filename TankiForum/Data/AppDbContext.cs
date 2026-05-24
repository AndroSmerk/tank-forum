using Microsoft.EntityFrameworkCore;
using TankiForum.Models;

namespace TankiForum.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<ForumCategory> ForumCategories => Set<ForumCategory>();
    public DbSet<ForumSection> ForumSections => Set<ForumSection>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Clan> Clans => Set<Clan>();
    public DbSet<UserClan> UserClans => Set<UserClan>();
    public DbSet<BlockedUser> BlockedUsers => Set<BlockedUser>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
