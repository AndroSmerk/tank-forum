using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TankiForum.Models;

namespace TankiForum.Data.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Avatar)
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("User");

        builder.Property(u => u.Vocation)
            .HasMaxLength(100);

        builder.Property(u => u.City)
            .HasMaxLength(100);

        builder.Property(u => u.BanReason)
            .HasMaxLength(500);

        builder.Property(u => u.Achievements)
            .HasMaxLength(2000);

        builder.Property(u => u.Medals)
            .HasMaxLength(2000);

        builder.HasMany(u => u.BlockedUsers)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
