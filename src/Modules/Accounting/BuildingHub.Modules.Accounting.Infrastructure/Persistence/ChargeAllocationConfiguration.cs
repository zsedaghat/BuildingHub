using BuildingHub.Modules.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Accounting.Infrastructure.Persistence;

public class ChargeAllocationConfiguration
    : IEntityTypeConfiguration<ChargeAllocation>
{
    public void Configure(
        EntityTypeBuilder<ChargeAllocation> builder)
    {
        builder.ToTable("ChargeAllocations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasIndex(x => new
        {
            x.ChargeId,
            x.UnitId
        })
        .IsUnique();

        builder.HasOne<Charge>()
            .WithMany()
            .HasForeignKey(x => x.ChargeId)
            .OnDelete(DeleteBehavior.Cascade);

       
    }
}