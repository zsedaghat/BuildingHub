
using BuildingHub.Modules.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Community.Infrastructure.Persistence;

public sealed class FloorConfiguration
    : IEntityTypeConfiguration<Floor>
{
    public void Configure(EntityTypeBuilder<Floor> builder)
    {
        builder.ToTable("floor");

        builder.HasKey(x => x.Id);

        builder.HasAlternateKey(x => new { x.Id, x.BuildingId })
    .HasName("ak_floor_id_building_id");

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.BuildingId)
            .HasColumnName("building_id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FloorNumber)
            .HasColumnName("floor_number")
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .HasColumnName("display_order")
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.BuildingId,
            x.FloorNumber
        })
            .IsUnique()
            .HasDatabaseName(
                "ux_floor_building_floor_number");

        builder.HasOne<Building>()
            .WithMany()
            .HasForeignKey(x => x.BuildingId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_floor_building");
    }
}
