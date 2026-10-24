
using BuildingHub.Modules.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Accounting.Infrastructure.Persistence;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UnitChargeId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.PaidAt)
            .IsRequired();

        builder.Property(x => x.TrackingNumber)
            .HasMaxLength(100);

        builder.HasOne<UnitCharge>()
            .WithMany()
            .HasForeignKey(x => x.UnitChargeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UnitChargeId);
    }
}
