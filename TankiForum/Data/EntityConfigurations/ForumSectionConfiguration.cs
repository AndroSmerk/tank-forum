using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TankiForum.Models;

namespace TankiForum.Data.EntityConfigurations;

public class ForumSectionConfiguration : IEntityTypeConfiguration<ForumSection>
{
    public void Configure(EntityTypeBuilder<ForumSection> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.HasOne(s => s.Category)
            .WithMany(c => c.Sections)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
