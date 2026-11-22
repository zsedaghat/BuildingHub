
using CommunityEntity =
    BuildingHub.Modules.Community.Domain.Entities.Community;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Community.Infrastructure.Persistence;

public sealed class CommunityModelConfiguration
    : IEntityTypeConfiguration<CommunityEntity>
{
    public void Configure(
        EntityTypeBuilder<CommunityEntity> builder)
    {
        builder.ToTable("community");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);
    }
}
