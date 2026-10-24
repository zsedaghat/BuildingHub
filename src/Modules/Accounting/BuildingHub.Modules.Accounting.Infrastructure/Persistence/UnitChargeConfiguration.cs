
using BuildingHub.Modules.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Accounting.Infrastructure.Persistence;

public class UnitChargeConfiguration
    : IEntityTypeConfiguration<UnitCharge>
{
    public void Configure(EntityTypeBuilder<UnitCharge> builder)
    {
        builder.ToTable("UnitCharges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChargeId)
            .IsRequired();

        builder.Property(x => x.UnitId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.ChargeId,
            x.UnitId
        }).IsUnique();

        builder.HasOne<Charge>()
            .WithMany()
            .HasForeignKey(x => x.ChargeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
