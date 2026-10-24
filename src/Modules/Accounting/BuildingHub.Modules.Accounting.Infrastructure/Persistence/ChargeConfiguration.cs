
using BuildingHub.Modules.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Accounting.Infrastructure.Persistence;

public class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    public void Configure(EntityTypeBuilder<Charge> builder)
    {
        builder.ToTable("Charges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.CalculationMethod)
            .IsRequired();

        builder.Property(x => x.Category)
            .IsRequired();

        builder.Property(x => x.PeriodYear)
            .IsRequired();

        builder.Property(x => x.PeriodMonth)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.BuildingId,
            x.PeriodYear,
            x.PeriodMonth
        });

        //builder.HasOne<BuildingHub.Modules.Community.Domain.Entities.Building>()
        //    .WithMany()
        //    .HasForeignKey(x => x.BuildingId)
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}
