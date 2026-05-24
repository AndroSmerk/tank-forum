using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TankiForum.Models;

namespace TankiForum.Data.EntityConfigurations;

public class UserClanConfiguration : IEntityTypeConfiguration<UserClan>
{
    public void Configure(EntityTypeBuilder<UserClan> builder)
    {
        builder.HasKey(uc => new { uc.UserId, uc.ClanId });

        builder.HasOne(uc => uc.User)
            .WithMany(u => u.Clans)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uc => uc.Clan)
            .WithMany(c => c.UserClans)
            .HasForeignKey(uc => uc.ClanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
