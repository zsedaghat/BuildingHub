
using BuildingHub.Modules.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Community.Infrastructure.Persistence;

public sealed class UnitPersonConfiguration
    : IEntityTypeConfiguration<UnitPerson>
{
    public void Configure(
        EntityTypeBuilder<UnitPerson> builder)
    {
        builder.ToTable("unit_person");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.UnitId)
            .HasColumnName("unit_id")
            .IsRequired();

        builder.Property(x => x.PersonId)
            .HasColumnName("person_id")
            .IsRequired();

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsPrimary)
            .HasColumnName("is_primary")
            .IsRequired();

        builder.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("date");

        builder.Property(x => x.EndDate)
            .HasColumnName("end_date")
            .HasColumnType("date");

        builder.HasCheckConstraint(
            "ck_unit_person_dates",
            "end_date IS NULL OR start_date IS NULL OR end_date >= start_date");

        builder.HasIndex(x => new
        {
            x.UnitId,
            x.PersonId,
            x.Role
        })
        .IsUnique()
        .HasDatabaseName("ux_unit_person_role");

        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_unit_person_unit");

        builder.HasOne<Person>()
            .WithMany()
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_unit_person_person");
    }
}
