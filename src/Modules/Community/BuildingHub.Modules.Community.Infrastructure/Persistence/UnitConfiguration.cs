
using BuildingHub.Modules.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Community.Infrastructure.Persistence;

public sealed class UnitConfiguration
    : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("unit");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.BuildingId)
            .HasColumnName("building_id")
            .IsRequired();

        builder.Property(x => x.FloorId)
            .HasColumnName("floor_id")
            .IsRequired(false);

        builder.Property(x => x.UnitNumber)
            .HasColumnName("unit_number")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Area)
            .HasColumnName("area")
            .HasPrecision(18, 2);

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.BuildingId,
            x.UnitNumber
        })
        .IsUnique()
        .HasDatabaseName("ux_unit_building_number");

        builder.HasOne<Building>()
            .WithMany()
            .HasForeignKey(x => x.BuildingId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_unit_building");

        builder.HasOne<Floor>()
    .WithMany()
    .HasForeignKey(x => new { x.FloorId, x.BuildingId })
    .HasPrincipalKey(x => new { x.Id, x.BuildingId })
    .OnDelete(DeleteBehavior.Restrict)
    .HasConstraintName("fk_unit_floor_building");
    }
}
