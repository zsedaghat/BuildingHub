using BuildingHub.Modules.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Community.Infrastructure.Persistence;

public class UnitPersonConfiguration : IEntityTypeConfiguration<UnitPerson>
{
    public void Configure(EntityTypeBuilder<UnitPerson> builder)
    {
        builder.ToTable("UnitPersons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.IsPrimary)
            .IsRequired();

        builder.Property(x => x.StartDate)
      .HasColumnType("date");

        builder.Property(x => x.EndDate)
            .HasColumnType("date");

        builder.HasIndex(x => new
        {
            x.UnitId,
            x.PersonId,
            x.Role
        })
   .IsUnique();

        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Person>()
            .WithMany()
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}