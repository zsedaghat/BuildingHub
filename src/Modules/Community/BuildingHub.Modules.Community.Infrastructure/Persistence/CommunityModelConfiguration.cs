using BuildingHub.Modules.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Community.Infrastructure.Persistence;

public class CommunityModelConfiguration : IEntityTypeConfiguration<BuildingHub.Modules.Community.Domain.Entities.Community>
{
    public void Configure(EntityTypeBuilder<BuildingHub.Modules.Community.Domain.Entities.Community> builder)
    {
        builder.ToTable("Communities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);
    }
}