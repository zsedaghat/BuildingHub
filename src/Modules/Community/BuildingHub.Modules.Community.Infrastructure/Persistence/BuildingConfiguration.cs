using BuildingHub.Modules.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Community.Infrastructure.Persistence;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("Buildings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.HasIndex(x => new { x.CommunityId, x.Name })
            .IsUnique();

        builder.HasOne<BuildingHub.Modules.Community.Domain.Entities.Community>()
            .WithMany()
            .HasForeignKey(x => x.CommunityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}